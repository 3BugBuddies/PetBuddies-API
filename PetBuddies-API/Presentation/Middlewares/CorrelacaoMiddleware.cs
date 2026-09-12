using Serilog.Context;
using System.Diagnostics;

namespace PetBuddies_API.Presentation.Middlewares
{
    /// <summary>
    /// Correlaciona as linhas de log de uma requisicao pelo identificador do rastreamento.
    /// </summary>
    public class CorrelacaoMiddleware
    {
        public const string CabecalhoCorrelacao = "X-Correlation-Id";

        private static readonly IDisposable SemEscopo = new EscopoVazio();

        private readonly RequestDelegate _proximo;
        private readonly ILogger<CorrelacaoMiddleware> _logger;

        public CorrelacaoMiddleware(RequestDelegate proximo, ILogger<CorrelacaoMiddleware> logger)
        {
            _proximo = proximo;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            var identificador = Activity.Current?.TraceId.ToString();

            if (string.IsNullOrEmpty(identificador))
            {
                // Fallback barulhento: sem identificador de rastreamento, avisa em vez de seguir em silencio.
                identificador = contexto.TraceIdentifier;
                _logger.LogWarning(
                    "Rastreamento indisponivel em {Caminho}: a correlacao caiu para o identificador do servidor",
                    contexto.Request.Path.Value);
            }

            // Escrito antes de chamar o proximo: depois que o corpo comeca a sair,
            // cabecalho novo nao entra mais.
            contexto.Response.Headers[CabecalhoCorrelacao] = identificador;

            // O valor que o cliente mandou entra como propriedade adicional e nunca
            // substitui o do rastreamento.
            var correlacaoDoCliente = contexto.Request.Headers[CabecalhoCorrelacao].ToString();

            var cronometro = Stopwatch.StartNew();

            using (LogContext.PushProperty("CorrelationId", identificador))
            using (EscopoDoCliente(correlacaoDoCliente))
            {
                var falhou = false;

                try
                {
                    await _proximo(contexto);
                }
                catch
                {
                    // Marca e relanca — nao trata.
                    falhou = true;
                    throw;
                }
                finally
                {
                    cronometro.Stop();

                    // Sem a marca, excecao nao tratada logaria "respondeu 200" para um 500 real.
                    var status = falhou && !contexto.Response.HasStarted
                        ? StatusCodes.Status500InternalServerError
                        : contexto.Response.StatusCode;

                    var nivel = status switch
                    {
                        >= 500 => LogLevel.Error,
                        >= 400 => LogLevel.Warning,
                        _ => LogLevel.Information
                    };

                    _logger.Log(
                        nivel,
                        "{Metodo} {Caminho} respondeu {StatusCode} em {DuracaoMs:F1} ms",
                        contexto.Request.Method,
                        contexto.Request.Path.Value,
                        status,
                        cronometro.Elapsed.TotalMilliseconds);
                }
            }
        }

        private static IDisposable EscopoDoCliente(string? valor) =>
            string.IsNullOrWhiteSpace(valor)
                ? SemEscopo
                : LogContext.PushProperty("ClientCorrelationId", valor);

        private sealed class EscopoVazio : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
