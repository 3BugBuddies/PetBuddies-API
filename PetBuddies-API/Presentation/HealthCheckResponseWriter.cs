using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace PetBuddies_API.Presentation
{
    /// <summary>
    /// Escreve o corpo JSON das rotas de saude: status agregado e uma linha por verificacao.
    /// </summary>
    public static class HealthCheckResponseWriter
    {
        private static readonly JsonSerializerOptions Opcoes = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static Task EscreverAsync(HttpContext contexto, HealthReport relatorio)
        {
            contexto.Response.ContentType = "application/json; charset=utf-8";

            var corpo = new
            {
                status = relatorio.Status.ToString(),
                checks = relatorio.Entries.Select(entrada => new
                {
                    nome = entrada.Key,
                    status = entrada.Value.Status.ToString(),
                    descricao = entrada.Value.Description ?? entrada.Value.Exception?.Message
                })
            };

            return contexto.Response.WriteAsync(JsonSerializer.Serialize(corpo, Opcoes));
        }
    }
}
