using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore.Infrastructure;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace PetBuddies_API.Infrastructure.IoC
{
    public static class Bootstrap
    {
        public static void AddTelemetria(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ApplicationInsights:ConnectionString"];

            // Sem connection string o UseAzureMonitor lanca na subida.
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            services.AddOpenTelemetry()
                .UseAzureMonitor(options =>
                {
                    options.ConnectionString = connectionString;
                });
        }

        public static void AddLogApi(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    "logs/api-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
                .CreateLogger();
        }

        public static void AddIoC(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseOracle(
                    configuration.GetConnectionString("Oracle"),
                    o => o.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
                );
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IProtocoloRepository, ProtocoloRepository>();
            services.AddScoped<IRegraProtocoloRepository, RegraProtocoloRepository>();
            services.AddScoped<IOfertaRepository, OfertaRepository>();
            services.AddScoped<IRegraPontuacaoRepository, RegraPontuacaoRepository>();

            services.AddScoped<IProtocoloService, ProtocoloService>();
            services.AddScoped<IRegraProtocoloService, RegraProtocoloService>();
            services.AddScoped<IOfertaService, OfertaService>();
            services.AddScoped<IRegraPontuacaoService, RegraPontuacaoService>();
        }
    }
}
