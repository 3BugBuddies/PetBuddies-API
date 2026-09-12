using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetBuddies_API.Presentation.Controllers
{
    [Route("api/health")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        // Liveness
        [HttpGet("live")]
        public Task<IActionResult> Live(CancellationToken ct) => Relatorio("live", ct);

        // Readiness
        [HttpGet("db")]
        public Task<IActionResult> Db(CancellationToken ct) => Relatorio("db", ct);

        [HttpGet("externo")]
        public Task<IActionResult> Externo(CancellationToken ct) => Relatorio("externo", ct);

        private async Task<IActionResult> Relatorio(string tag, CancellationToken ct)
        {
            var report = await _healthCheckService.CheckHealthAsync(r => r.Tags.Contains(tag), ct);

            var result = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    error = e.Value.Exception?.Message
                })
            };

            return report.Status == HealthStatus.Unhealthy
                ? StatusCode(StatusCodes.Status503ServiceUnavailable, result)
                : Ok(result);
        }
    }
}
