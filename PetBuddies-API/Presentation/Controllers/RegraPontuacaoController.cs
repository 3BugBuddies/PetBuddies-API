using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Presentation.Controllers
{
    [ApiController]
    [Route("api/regra-pontuacao")]
    [Authorize(Roles = "VET")]
    public class RegraPontuacaoController : ControllerBase
    {
        private readonly IRegraPontuacaoService _regraPontuacaoService;

        public RegraPontuacaoController(IRegraPontuacaoService regraPontuacaoService)
        {
            _regraPontuacaoService = regraPontuacaoService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista regras de pontuação, da vigência mais recente para a mais antiga")]
        [SwaggerResponse(StatusCodes.Status200OK, "Regras listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma regra encontrada.")]
        public async Task<ActionResult<List<RegraPontuacaoDto>>> Listar(
            [FromQuery] int? clinicaId,
            [FromQuery] TipoGestoEnum? gesto,
            CancellationToken cancellationToken)
        {
            var response = await _regraPontuacaoService.ListarAsync(clinicaId, gesto, cancellationToken);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca regra de pontuação por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Regra encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Regra não encontrada.")]
        public async Task<ActionResult<RegraPontuacaoDto>> BuscarPorId(int id, CancellationToken cancellationToken)
        {
            var response = await _regraPontuacaoService.BuscarPorIdAsync(id, cancellationToken);
            return response is null
                ? NotFound("Regra de pontuação não encontrada para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra regra de pontuação")]
        [SwaggerResponse(StatusCodes.Status201Created, "Regra cadastrada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Já existe valor para essa clínica, gesto e vigência.")]
        public async Task<ActionResult<RegraPontuacaoDto>> Cadastrar(
            [FromBody] SalvarRegraPontuacaoRequest request,
            CancellationToken cancellationToken)
        {
            var validacao = await ValidarAsync(request, null, cancellationToken);
            if (validacao is not null)
            {
                return validacao;
            }

            var response = await _regraPontuacaoService.CadastrarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza regra de pontuação")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Regra atualizada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Regra não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Já existe valor para essa clínica, gesto e vigência.")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] SalvarRegraPontuacaoRequest request,
            CancellationToken cancellationToken)
        {
            var validacao = await ValidarAsync(request, id, cancellationToken);
            if (validacao is not null)
            {
                return validacao;
            }

            var atualizada = await _regraPontuacaoService.AtualizarAsync(id, request, cancellationToken);
            return atualizada is null
                ? NotFound("Regra de pontuação não encontrada para o id informado.")
                : NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove regra de pontuação")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Regra removida com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Regra não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Regra possui vínculos e não pode ser removida.")]
        public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
        {
            try
            {
                var removida = await _regraPontuacaoService.RemoverAsync(id, cancellationToken);
                return removida
                    ? NoContent()
                    : NotFound("Regra de pontuação não encontrada para o id informado.");
            }
            catch (DbUpdateException)
            {
                return Conflict("Regra de pontuação possui vínculos e não pode ser removida.");
            }
        }

        private async Task<ActionResult?> ValidarAsync(
            SalvarRegraPontuacaoRequest request,
            int? ignorarRegraId,
            CancellationToken cancellationToken)
        {
            var violacao = _regraPontuacaoService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            if (await _regraPontuacaoService.VigenciaExisteAsync(request, ignorarRegraId, cancellationToken))
            {
                return Conflict("Já existe valor para essa clínica, gesto e vigência.");
            }

            return null;
        }
    }
}
