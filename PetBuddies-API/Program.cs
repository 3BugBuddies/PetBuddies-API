using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Clients;
using PetBuddies_API.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

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
