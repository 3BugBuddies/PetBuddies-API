using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Presentation.Controllers
{
    /// <summary>
    /// As regras de um protocolo: o molde de cada item que o motor materializa.
    /// </summary>
    [ApiController]
    [Route("api/regras-protocolo")]
    [Authorize]
    public class RegraProtocoloController : ControllerBase
    {
        private readonly IRegraProtocoloService _regraProtocoloService;

        public RegraProtocoloController(IRegraProtocoloService regraProtocoloService)
        {
            _regraProtocoloService = regraProtocoloService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista as regras de um protocolo")]
        [SwaggerResponse(StatusCodes.Status200OK, "Regras listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma regra encontrada.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "protocoloId não informado.")]
        public async Task<ActionResult<List<RegraProtocoloDto>>> Listar(
            [FromQuery] long? protocoloId,
            CancellationToken cancellationToken)
        {
            // Sem o filtro a rota devolveria a tabela inteira; exigir o pai e mais honesto
            // que deixar o binder assumir zero e responder 204.
            if (protocoloId is null)
            {
                return BadRequest("Informe protocoloId para listar as regras.");
            }

            var response = await _regraProtocoloService.ListarPorProtocoloAsync(protocoloId.Value, cancellationToken);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Busca regra de protocolo por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Regra encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Regra não encontrada.")]
        public async Task<ActionResult<RegraProtocoloDto>> BuscarPorId(long id, CancellationToken cancellationToken)
        {
            var response = await _regraProtocoloService.BuscarPorIdAsync(id, cancellationToken);
            return response is null
                ? NotFound("Regra de protocolo não encontrada para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra regra de protocolo")]
        [SwaggerResponse(StatusCodes.Status201Created, "Regra cadastrada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Protocolo não encontrado.")]
        public async Task<ActionResult<RegraProtocoloDto>> Cadastrar(
            [FromBody] SalvarRegraProtocoloRequest request,
            CancellationToken cancellationToken)
        {
            var violacao = _regraProtocoloService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            if (!await _regraProtocoloService.ProtocoloExisteAsync(request.ProtocoloId, cancellationToken))
            {
                return NotFound("Protocolo não encontrado para cadastrar a regra.");
            }

            var response = await _regraProtocoloService.CadastrarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(Summary = "Atualiza regra de protocolo")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Regra atualizada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Regra ou protocolo não encontrado.")]
        public async Task<IActionResult> Atualizar(
            long id,
            [FromBody] SalvarRegraProtocoloRequest request,
            CancellationToken cancellationToken)
        {
            var violacao = _regraProtocoloService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            if (!await _regraProtocoloService.ProtocoloExisteAsync(request.ProtocoloId, cancellationToken))
            {
                return NotFound("Protocolo não encontrado para a regra.");
            }

            var atualizada = await _regraProtocoloService.AtualizarAsync(id, request, cancellationToken);
            return atualizada is null
                ? NotFound("Regra de protocolo não encontrada para o id informado.")
                : NoContent();
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(Summary = "Remove regra de protocolo")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Regra removida com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Regra não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Regra possui vínculos e não pode ser removida.")]
        public async Task<IActionResult> Remover(long id, CancellationToken cancellationToken)
        {
            try
            {
                var removida = await _regraProtocoloService.RemoverAsync(id, cancellationToken);
                return removida
                    ? NoContent()
                    : NotFound("Regra de protocolo não encontrada para o id informado.");
            }
            catch (DbUpdateException)
            {
                return Conflict("Regra de protocolo possui vínculos e não pode ser removida.");
            }
        }
    }
}
