using Serilog.Context;
using System.Diagnostics;

namespace PetBuddies_API.Presentation.Middlewares
{
    /// <summary>
    /// Correlaciona as linhas de log de uma requisicao pelo identificador que o
    /// rastreamento ja criou (decisao AN3 = A): nao se inventa identificador.
    /// E o unico middleware do servico nesta sprint — o padrao de erros simples
    /// no controller continua, e nao existe handler global de excecao.
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
                // Caminho barulhento de proposito: sem o rastreamento registrado
                // antes deste middleware a correlacao ficaria sem identificador
                // em silencio, que e a armadilha que o PR precisa evitar.
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
                try
                {
                    await _proximo(contexto);
                }
                finally
                {
                    cronometro.Stop();
                    _logger.LogInformation(
                        "{Metodo} {Caminho} respondeu {StatusCode} em {DuracaoMs:F1} ms",
                        contexto.Request.Method,
                        contexto.Request.Path.Value,
                        contexto.Response.StatusCode,
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
