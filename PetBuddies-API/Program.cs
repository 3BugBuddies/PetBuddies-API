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
using PetBuddies_API.Infrastructure.Clients;
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
builder.Services.AddHttpClient();
// Confirmacao da unidade de trabalho: quem chama SalvarAsync e o caso de uso,
// nunca o repositorio — e o que permite o fechamento do N7 ser atomico.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Cliente do servico de cuidado (Java), por interface para poder ser dublado.
builder.Services.AddScoped<IMotorApiClient, MotorApiClient>();

// Repositorios — Domain/Interfaces -> Infrastructure/Repositories
builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<IClinicaRepository, ClinicaRepository>();
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IJanelaAtendimentoRepository, JanelaAtendimentoRepository>();
builder.Services.AddScoped<IProcedimentoRepository, ProcedimentoRepository>();
builder.Services.AddScoped<IRegistroAtendimentoRepository, RegistroAtendimentoRepository>();
builder.Services.AddScoped<IResponsavelRepository, ResponsavelRepository>();
builder.Services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();

// Back-office da clinica (N11): catalogo de cuidado e politica comercial.
builder.Services.AddScoped<IProtocoloRepository, ProtocoloRepository>();
builder.Services.AddScoped<IRegraProtocoloRepository, RegraProtocoloRepository>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<IRegraPontuacaoRepository, RegraPontuacaoRepository>();

// Servicos de aplicacao — Application/Interfaces -> Application/UseCases
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IAnimalMotorService, AnimalMotorService>();
builder.Services.AddScoped<IClinicaService, ClinicaService>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();
builder.Services.AddScoped<IJanelaAtendimentoService, JanelaAtendimentoService>();
builder.Services.AddScoped<IProcedimentoService, ProcedimentoService>();
builder.Services.AddScoped<IRegistroAtendimentoService, RegistroAtendimentoService>();
builder.Services.AddScoped<IResponsavelService, ResponsavelService>();
builder.Services.AddScoped<IVeterinarioService, VeterinarioService>();

builder.Services.AddScoped<IProtocoloService, ProtocoloService>();
builder.Services.AddScoped<IRegraProtocoloService, RegraProtocoloService>();
builder.Services.AddScoped<IOfertaService, OfertaService>();
builder.Services.AddScoped<IRegraPontuacaoService, RegraPontuacaoService>();

// ---------------------------------------------------------------------------
// Autenticacao: o token e emitido pelo Java e apenas VALIDADO aqui (ADR s3-20).
// HS256 com segredo simetrico, conferindo assinatura, exp e iss.
//
// O segredo vem da variavel de ambiente PETBUDDIES_JWT_SECRET, nunca do
// appsettings versionado — credencial no fonte custa 20 pontos na frente de
// DevOps. ValidateOnStart derruba a subida se ela faltar; o design-time do
// EF nao chega la, porque para no Build().
//
// Identity nao entra: o PDF so o pede na Sprint 4.
// ---------------------------------------------------------------------------
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
            // O s3-20 nao define claim aud; validar audiencia rejeitaria todo token do Java.
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            // O perfil (VET | TUTOR) vira role, e autorizacao por rota fica a um atributo.
            RoleClaimType = "perfil"
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// CORS: sem ele o painel web nao fala com esta API, e o erro so aparece no
// browser. Origens vem da configuracao — nada de AllowAnyOrigin com credencial.
// ---------------------------------------------------------------------------
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

    // Sem isto o Swagger nao tem onde colar o token, e os endpoints do back-office
    // ficam intestaveis pela UI depois que ganharam [Authorize].
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

// Saude — tres verificacoes, quatro rotas. "self" nao toca em nada: e ele que
// distingue "processo caiu" de "banco caiu". A cadeia do Oracle pode nem existir
// (Testing nao carrega appsettings.Development.json); nesse caso a verificacao
// falha e reporta indisponivel, que e a resposta correta.
// O timeout de tres segundos evita que um monitor batendo de segundo em segundo
// segure o pool, que a cadeia ja limita a tres conexoes.
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

// Rastreamento e metricas (decisao N3 = A). As tres instrumentacoes sao o que
// produz os spans: ASP.NET Core da o span do controlador, HttpClient o da chamada
// ao Java, e Entity Framework Core o do banco. As metricas de ASP.NET Core dao
// duracao da requisicao e contagem por codigo de status — o tempo de resposta e a
// taxa de erro que a rubrica pede.
//
// Sem coletor OTLP configurado o exportador e o console: e a unica forma de ver
// span rodando local, onde nao ha coletor nenhum. Com OTEL_EXPORTER_OTLP_ENDPOINT
// definido, a saida vai para o coletor.
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

// Gancho da Sprint 4: a chave e lida da configuracao e nao usada. O valor nunca
// entra em linha de log — so o fato de existir ou nao.
var chaveApplicationInsights = builder.Configuration["ApplicationInsights:ConnectionString"];
Log.Information(
    "Application Insights {Estado} — gancho da Sprint 4, lido e nao usado nesta sprint",
    string.IsNullOrWhiteSpace(chaveApplicationInsights) ? "nao configurado" : "configurado");

var app = builder.Build();

// Em Testing a WebApplicationFactory sobe este mesmo Program: migrar aqui faria
// todo teste de integracao abrir o Oracle. Fora de Testing a migracao continua,
// porque com bancos separados (S1 = B) ela e a dona daquele schema.
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    db.Database.Migrate();
}

// Primeiro middleware do pipeline: toda linha de log da requisicao — inclusive as
// do Swagger e das rotas de saude — nasce dentro do escopo da correlacao.
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

// A ordem importa: CORS antes de autenticar, e autenticar antes de autorizar.
// Sem UseAuthentication, [Authorize] devolve 500 em vez de 401.
app.UseCors(PoliticaCorsDoPainel);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// As quatro rotas de saude nao exigem token e precisam continuar assim quando a
// validacao do token (N4) entrar — por isso o AllowAnonymous explicito agora.
// motor-java responde indisponivel ate o endereco de saude do Java (J5) existir:
// uma verificacao que reporta dependencia ausente como ausente esta funcionando.
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

// Necessario para WebApplicationFactory<Program> enxergar o tipo (PR N6).
public partial class Program { }
