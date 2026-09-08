using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.UseCases;
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
builder.Services.AddScoped<MotorApiClient>();
builder.Services.AddScoped<AnimalMotorService>();
builder.Services.AddScoped<AnimalCadastroService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<ClinicaService>();
builder.Services.AddScoped<JanelaAtendimentoService>();
builder.Services.AddScoped<ProcedimentoService>();
builder.Services.AddScoped<RegistroAtendimentoService>();
builder.Services.AddScoped<ResponsavelService>();
builder.Services.AddScoped<VeterinarioService>();


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
