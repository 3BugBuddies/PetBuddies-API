using System;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
            // Console sem coletor, OTLP quando OTEL_EXPORTER_OTLP_ENDPOINT existir.
            var exportarNoConsole = string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

            services.AddOpenTelemetry()
                .ConfigureResource(recurso => recurso.AddService(
                    serviceName: "petbuddies-api",
                    serviceVersion: "1.0.0"))
                .WithTracing(rastreamento =>
                {
                    rastreamento
                        // Health check e coleta de metrica sao trafego de infraestrutura:
                        // rastreados, afogam no console as requisicoes que interessam.
                        .AddAspNetCoreInstrumentation(opcoes => opcoes.Filter = contexto =>
                        {
                            var caminho = contexto.Request.Path.Value ?? string.Empty;
                            return !caminho.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
                                && !caminho.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase);
                        })
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

                    // Fora do if de proposito: dentro dele, /metrics responderia vazio
                    // sempre que houvesse coletor configurado.
                    metricas.AddPrometheusExporter();

                    // Metrica nao vai para o console: o despejo periodico era metade do
                    // log, e /metrics entrega o mesmo dado quando alguem pede.
                    if (!exportarNoConsole)
                    {
                        metricas.AddOtlpExporter();
                    }
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

            services.AddTransient<IProtocoloRepository, ProtocoloRepository>();
            services.AddTransient<IRegraProtocoloRepository, RegraProtocoloRepository>();
            services.AddTransient<IOfertaRepository, OfertaRepository>();
            services.AddTransient<IRegraPontuacaoRepository, RegraPontuacaoRepository>();

            services.AddTransient<IProtocoloService, ProtocoloService>();
            services.AddTransient<IRegraProtocoloService, RegraProtocoloService>();
            services.AddTransient<IOfertaService, OfertaService>();
            services.AddTransient<IRegraPontuacaoService, RegraPontuacaoService>();
        }
    }
}
