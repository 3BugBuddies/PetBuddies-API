using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Clients;
using PetBuddies_API.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
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

app.UseAuthorization();

app.MapControllers();

app.Run();

// Necessario para WebApplicationFactory<Program> enxergar o tipo (PR N6).
public partial class Program { }
