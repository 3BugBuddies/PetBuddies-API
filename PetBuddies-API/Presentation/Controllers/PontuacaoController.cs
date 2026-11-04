using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetBuddies_API.Application.Dtos.Pontuacao;
using PetBuddies_API.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Presentation.Controllers
{
    [ApiController]
    [Route("api/pontuacao")]
    [Authorize]
    [EnableRateLimiting(LimiteDeRequisicoes.Politica)]
    public class PontuacaoController : ControllerBase
    {
        // Mesmo nome de claim que o TokenService do Java escreve no token do tutor.
        public const string ClaimResponsavelId = "responsavelId";

        private readonly IPontuacaoService _pontuacaoService;

        public PontuacaoController(IPontuacaoService pontuacaoService)
        {
            _pontuacaoService = pontuacaoService;
        }

        [HttpPost("consulta")]
        [Authorize(Roles = "VET")]
        [SwaggerOperation(
            Summary = "Calcula e lança os pontos do responsável por uma consulta realizada",
            Description = "Chamado pelo Java ao fechar a consulta. Reenviar a mesma consulta devolve o lançamento existente, sem pontuar de novo.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Pontos lançados.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Consulta já pontuada; devolve o lançamento existente.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        public async Task<ActionResult<LancamentoPontosDto>> PontuarConsulta(
            [FromBody] PontuarConsultaRequest request,
            CancellationToken cancellationToken)
        {
            var violacao = _pontuacaoService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            var resultado = await _pontuacaoService.PontuarConsultaAsync(request, cancellationToken);

            return resultado.JaExistia
                ? Ok(resultado.Lancamento)
                : CreatedAtAction(nameof(ObterMemoria), new { consultaId = resultado.Lancamento.ConsultaId }, resultado.Lancamento);
        }

        [HttpGet("saldo")]
        [Authorize(Roles = "TUTOR")]
        [SwaggerOperation(Summary = "Saldo de pontos do responsável do token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Saldo calculado.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Token sem o vínculo responsavelId.")]
        public async Task<ActionResult<SaldoPontosDto>> ObterSaldo(CancellationToken cancellationToken)
        {
            var responsavelId = ResponsavelDoToken();
            if (responsavelId is null)
            {
                return SemVinculoDeResponsavel();
            }

            return Ok(await _pontuacaoService.ObterSaldoAsync(responsavelId.Value, cancellationToken));
        }

        [HttpGet("extrato")]
        [Authorize(Roles = "TUTOR")]
        [SwaggerOperation(Summary = "Lançamentos do responsável do token, da consulta mais recente para a mais antiga")]
        [SwaggerResponse(StatusCodes.Status200OK, "Lançamentos listados.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum lançamento.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Token sem o vínculo responsavelId.")]
        public async Task<ActionResult<List<LancamentoPontosDto>>> ListarExtrato(CancellationToken cancellationToken)
        {
            var responsavelId = ResponsavelDoToken();
            if (responsavelId is null)
            {
                return SemVinculoDeResponsavel();
            }

            var extrato = await _pontuacaoService.ListarExtratoAsync(responsavelId.Value, cancellationToken);
            return extrato.Count == 0 ? NoContent() : Ok(extrato);
        }

        [HttpGet("consulta/{consultaId:long}/memoria")]
        [Authorize(Roles = "VET,TUTOR")]
        [SwaggerOperation(
            Summary = "Memória de cálculo de uma consulta pontuada (MongoDB)",
            Description = "O veterinário vê qualquer consulta; o tutor, só as dele.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Memória encontrada.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Consulta de outro responsável.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Memória não encontrada.")]
        [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "MongoDB indisponível.")]
        public async Task<ActionResult<MemoriaCalculoDto>> ObterMemoria(long consultaId, CancellationToken cancellationToken)
        {
            MemoriaCalculoDto? memoria;
            try
            {
                memoria = await _pontuacaoService.ObterMemoriaAsync(consultaId, cancellationToken);
            }
            catch (TimeoutException)
            {
                // O driver do MongoDB lança TimeoutException quando não acha o servidor.
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Memória de cálculo indisponível no momento.");
            }

            if (memoria is null)
            {
                return NotFound("Memória de cálculo não encontrada para a consulta informada.");
            }

            if (User.IsInRole("TUTOR") && memoria.ResponsavelId != ResponsavelDoToken())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "A consulta é de outro responsável.");
            }

            return Ok(memoria);
        }

        private long? ResponsavelDoToken()
        {
            return long.TryParse(User.FindFirst(ClaimResponsavelId)?.Value, out var responsavelId)
                ? responsavelId
                : null;
        }

        private ObjectResult SemVinculoDeResponsavel()
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Token de tutor sem o vínculo responsavelId.");
        }
    }
}
