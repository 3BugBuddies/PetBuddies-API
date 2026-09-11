using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Repositories;
using PetBuddies_API.Infrastructure.Security;
using PetBuddies_API.Presentation;
using PetBuddies_API.Presentation.Middlewares;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Text;
using System.Text.Json.Serialization;

// Serilog e a primeira coisa que sobe. Configurado depois do host, as linhas da
// subida sairiam no logger padrao e nunca chegariam ao arquivo.
var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var configuracaoDoLog = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{ambiente}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

// Os niveis vem do codigo e podem ser sobrescritos pela secao Serilog do appsettings.
// O arquivo sai em JSON compacto: e nele que as propriedades enriquecidas — entre
// elas o CorrelationId — ficam legiveis por maquina.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(configuracaoDoLog)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        new CompactJsonFormatter(),
        "logs/api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

Log.Information("PetBuddies-API subindo no ambiente {Ambiente}", ambiente);

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

const string PoliticaCorsDoPainel = "painel-clinica";

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseOracle(
        builder.Configuration.GetConnectionString("Oracle"),
        o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
    );
});
// Confirmacao da unidade de trabalho: quem chama SalvarAsync e o caso de uso,
// nunca o repositorio.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Back-office da clinica
builder.Services.AddScoped<IProtocoloRepository, ProtocoloRepository>();
builder.Services.AddScoped<IRegraProtocoloRepository, RegraProtocoloRepository>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<IRegraPontuacaoRepository, RegraPontuacaoRepository>();

builder.Services.AddScoped<IProtocoloService, ProtocoloService>();
builder.Services.AddScoped<IRegraProtocoloService, RegraProtocoloService>();
builder.Services.AddScoped<IOfertaService, OfertaService>();
builder.Services.AddScoped<IRegraPontuacaoService, RegraPontuacaoService>();

// ValidateOnStart derruba a subida quando PETBUDDIES_JWT_SECRET falta; o
// design-time do EF nao chega la, porque para no Build().
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SecaoConfiguracao))
    .Configure(opcoes =>
    {
        var segredo = builder.Configuration[JwtOptions.VariavelDeAmbienteDoSegredo];
        if (!string.IsNullOrWhiteSpace(segredo))
        {
            opcoes.Secret = segredo;
        }
    })
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        var jwt = builder.Configuration.GetSection(JwtOptions.SecaoConfiguracao).Get<JwtOptions>() ?? new JwtOptions();
        var segredo = builder.Configuration[JwtOptions.VariavelDeAmbienteDoSegredo] ?? jwt.Secret;

        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(string.IsNullOrWhiteSpace(segredo)
                    ? new string('0', 32) // placeholder: ValidateOnStart ja barra a subida sem o segredo
                    : segredo)),
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = "perfil"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(opcoes =>
{
    var origens = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:5173" };

    opcoes.AddPolicy(PoliticaCorsDoPainel, politica => politica
        .WithOrigins(origens)
        .AllowAnyHeader()
        .AllowAnyMethod());
});


// serializa todos enums para string ao inves de number
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token emitido pelo servico Java (POST /api/auth/login)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Saude — self, oracle, motor-java
var cadeiaOracle = builder.Configuration.GetConnectionString("Oracle");
var urlDoMotor = (builder.Configuration["MotorApi:BaseUrl"] ?? "http://localhost:8080").TrimEnd('/');

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddOracle(
        string.IsNullOrWhiteSpace(cadeiaOracle) ? "Data Source=oracle-nao-configurado;" : cadeiaOracle,
        name: "oracle",
        tags: ["db"],
        timeout: TimeSpan.FromSeconds(3))
    .AddUrlGroup(
        new Uri($"{urlDoMotor}/actuator/health"),
        name: "motor-java",
        tags: ["externo"],
        timeout: TimeSpan.FromSeconds(3));

// Rastreamento e metricas — console sem coletor, OTLP quando OTEL_EXPORTER_OTLP_ENDPOINT existir.
var endpointOtlp = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
var exportarNoConsole = string.IsNullOrWhiteSpace(endpointOtlp);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(recurso => recurso.AddService(
        serviceName: "petbuddies-api",
        serviceVersion: "1.0.0"))
    .WithTracing(rastreamento =>
    {
        rastreamento
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();

        if (exportarNoConsole)
        {
            rastreamento.AddConsoleExporter();
        }
        else
        {
            rastreamento.AddOtlpExporter();
        }
    })
    .WithMetrics(metricas =>
    {
        metricas.AddAspNetCoreInstrumentation();

        if (exportarNoConsole)
        {
            metricas.AddConsoleExporter();
        }
        else
        {
            metricas.AddOtlpExporter();
        }
    });

// O valor nunca entra em linha de log — so o fato de existir ou nao.
var chaveApplicationInsights = builder.Configuration["ApplicationInsights:ConnectionString"];
Log.Information(
    "Application Insights {Estado}",
    string.IsNullOrWhiteSpace(chaveApplicationInsights) ? "nao configurado" : "configurado");

var app = builder.Build();

// Em Testing a WebApplicationFactory sobe este mesmo Program: migrar aqui abriria o Oracle em todo teste.
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    db.Database.Migrate();
}

// Primeiro middleware do pipeline: log da requisicao dentro do escopo da correlacao.
app.UseMiddleware<CorrelacaoMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(PoliticaCorsDoPainel);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Rotas de saude nao exigem token.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = verificacao => verificacao.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.EscreverAsync
}).AllowAnonymous();

app.MapHealthChecks("/health/db", new HealthCheckOptions
{
    Predicate = verificacao => verificacao.Tags.Contains("db"),
    ResponseWriter = HealthCheckResponseWriter.EscreverAsync
}).AllowAnonymous();

app.MapHealthChecks("/health/externo", new HealthCheckOptions
{
    Predicate = verificacao => verificacao.Tags.Contains("externo"),
    ResponseWriter = HealthCheckResponseWriter.EscreverAsync
}).AllowAnonymous();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.EscreverAsync
}).AllowAnonymous();

app.Run();

// Necessario para WebApplicationFactory<Program> enxergar o tipo.
public partial class Program { }
