using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Presentation.Controllers
{
    /// <summary>
    /// O catálogo de cuidado da clínica. É o único recurso deste serviço que outro serviço lê:
    /// o motor do Java consulta <c>GET /api/protocolos?especie=&amp;categoria=</c> no momento de
    /// criar um plano, e materializa os itens a partir das regras que vêm no corpo.
    /// </summary>
    [ApiController]
    [Route("api/protocolos")]
    [Authorize]
    public class ProtocoloController : ControllerBase
    {
        private readonly IProtocoloService _protocoloService;

        public ProtocoloController(IProtocoloService protocoloService)
        {
            _protocoloService = protocoloService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista protocolos do catálogo, com as regras de cada um")]
        [SwaggerResponse(StatusCodes.Status200OK, "Protocolos listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum protocolo encontrado.")]
        public async Task<ActionResult<List<ProtocoloDto>>> Listar(
            [FromQuery] EspecieEnum? especie,
            [FromQuery] CategoriaProtocoloEnum? categoria,
            [FromQuery] bool? ativo,
            CancellationToken cancellationToken)
        {
            var response = await _protocoloService.ListarAsync(especie, categoria, ativo, cancellationToken);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Busca protocolo por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Protocolo encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Protocolo não encontrado.")]
        public async Task<ActionResult<ProtocoloDto>> BuscarPorId(long id, CancellationToken cancellationToken)
        {
            var response = await _protocoloService.BuscarPorIdAsync(id, cancellationToken);
            return response is null
                ? NotFound("Protocolo não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra protocolo")]
        [SwaggerResponse(StatusCodes.Status201Created, "Protocolo cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        public async Task<ActionResult<ProtocoloDto>> Cadastrar(
            [FromBody] SalvarProtocoloRequest request,
            CancellationToken cancellationToken)
        {
            var violacao = _protocoloService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            var response = await _protocoloService.CadastrarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:long}")]
        [SwaggerOperation(Summary = "Atualiza protocolo")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Protocolo atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Protocolo não encontrado.")]
        public async Task<IActionResult> Atualizar(
            long id,
            [FromBody] SalvarProtocoloRequest request,
            CancellationToken cancellationToken)
        {
            var violacao = _protocoloService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            var atualizado = await _protocoloService.AtualizarAsync(id, request, cancellationToken);
            return atualizado is null
                ? NotFound("Protocolo não encontrado para o id informado.")
                : NoContent();
        }

        [HttpDelete("{id:long}")]
        [SwaggerOperation(Summary = "Remove protocolo")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Protocolo removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Protocolo não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Protocolo tem regras ou ofertas e não pode ser removido.")]
        public async Task<IActionResult> Remover(long id, CancellationToken cancellationToken)
        {
            try
            {
                var removido = await _protocoloService.RemoverAsync(id, cancellationToken);
                return removido
                    ? NoContent()
                    : NotFound("Protocolo não encontrado para o id informado.");
            }
            catch (DbUpdateException)
            {
                return Conflict("Protocolo tem regras ou ofertas vinculadas e não pode ser removido.");
            }
        }
    }
}
