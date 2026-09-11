using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.IoC;
using PetBuddies_API.Infrastructure.Security;
using PetBuddies_API.Presentation;
using PetBuddies_API.Presentation.Middlewares;
using Serilog;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

// Serilog e a primeira coisa que sobe. Configurado depois do host, as linhas da
// subida sairiam no logger padrao e nunca chegariam ao arquivo.
var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var configuracaoDoLog = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{ambiente}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

Bootstrap.AddLogApi(configuracaoDoLog);

Log.Information("PetBuddies-API subindo no ambiente {Ambiente}", ambiente);

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

const string PoliticaCorsDoPainel = "painel-clinica";

Bootstrap.AddIoC(builder.Services, builder.Configuration);

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

//Adicionar compressao de dados
builder.Services.AddResponseCompression(options =>
{
    //br - Brotli
    options.Providers.Add<BrotliCompressionProvider>();
    //gzip
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// Add Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "politica_5_tentativas", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(20);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

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

// Tracing e metricas — Application Insights
builder.Services.AddTelemetria(builder.Configuration);

// O valor nunca entra em linha de log — so o fato de existir ou nao.
Log.Information(
    "Application Insights {Estado}",
    string.IsNullOrWhiteSpace(builder.Configuration["ApplicationInsights:ConnectionString"])
        ? "nao configurado"
        : "configurado");

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

app.UseRateLimiter();
app.UseResponseCompression();

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
