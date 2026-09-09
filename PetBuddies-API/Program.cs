using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Clients;
using PetBuddies_API.Infrastructure.Repositories;
using PetBuddies_API.Infrastructure.Security;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();

// Necessario para WebApplicationFactory<Program> enxergar o tipo (PR N6).
public partial class Program { }
