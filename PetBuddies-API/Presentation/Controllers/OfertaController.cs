using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Presentation.Controllers
{
    /// <summary>
    /// O que a clínica cobra, e desde quando. A vigência é por sucessão: cadastrar uma oferta
    /// nova para o mesmo alvo não edita a anterior — o preço antigo vale até o dia em que o
    /// novo começa.
    /// </summary>
    [ApiController]
    [Route("api/ofertas")]
    [Authorize]
    public class OfertaController : ControllerBase
    {
        private readonly IOfertaService _ofertaService;

        public OfertaController(IOfertaService ofertaService)
        {
            _ofertaService = ofertaService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista ofertas, da vigência mais recente para a mais antiga")]
        [SwaggerResponse(StatusCodes.Status200OK, "Ofertas listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma oferta encontrada.")]
        public async Task<ActionResult<List<OfertaDto>>> Listar(
            [FromQuery] int? clinicaId,
            [FromQuery] TipoAtoOfertaEnum? ato,
            CancellationToken cancellationToken)
        {
            var response = await _ofertaService.ListarAsync(clinicaId, ato, cancellationToken);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca oferta por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Oferta encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Oferta não encontrada.")]
        public async Task<ActionResult<OfertaDto>> BuscarPorId(int id, CancellationToken cancellationToken)
        {
            var response = await _ofertaService.BuscarPorIdAsync(id, cancellationToken);
            return response is null
                ? NotFound("Oferta não encontrada para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra oferta")]
        [SwaggerResponse(StatusCodes.Status201Created, "Oferta cadastrada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos ou alvo incoerente com o ato.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Protocolo não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Já existe oferta desse alvo começando nessa data.")]
        public async Task<ActionResult<OfertaDto>> Cadastrar(
            [FromBody] SalvarOfertaRequest request,
            CancellationToken cancellationToken)
        {
            var validacao = await ValidarAsync(request, null, cancellationToken);
            if (validacao is not null)
            {
                return validacao;
            }

            var response = await _ofertaService.CadastrarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza oferta")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Oferta atualizada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos ou alvo incoerente com o ato.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Oferta ou protocolo não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Já existe oferta desse alvo começando nessa data.")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] SalvarOfertaRequest request,
            CancellationToken cancellationToken)
        {
            var validacao = await ValidarAsync(request, id, cancellationToken);
            if (validacao is not null)
            {
                return validacao;
            }

            var atualizada = await _ofertaService.AtualizarAsync(id, request, cancellationToken);
            return atualizada is null
                ? NotFound("Oferta não encontrada para o id informado.")
                : NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove oferta")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Oferta removida com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Oferta não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Oferta possui vínculos e não pode ser removida.")]
        public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
        {
            try
            {
                var removida = await _ofertaService.RemoverAsync(id, cancellationToken);
                return removida
                    ? NoContent()
                    : NotFound("Oferta não encontrada para o id informado.");
            }
            catch (DbUpdateException)
            {
                return Conflict("Oferta possui vínculos e não pode ser removida.");
            }
        }

        private async Task<ActionResult?> ValidarAsync(
            SalvarOfertaRequest request,
            int? ignorarOfertaId,
            CancellationToken cancellationToken)
        {
            var violacao = _ofertaService.Validar(request);
            if (violacao is not null)
            {
                return BadRequest(violacao);
            }

            if (request.Ato == TipoAtoOfertaEnum.PROTOCOLO
                && !await _ofertaService.ProtocoloExisteAsync(request.ProtocoloId!.Value, cancellationToken))
            {
                return NotFound("Protocolo não encontrado para a oferta.");
            }

            if (await _ofertaService.VigenciaExisteAsync(request, ignorarOfertaId, cancellationToken))
            {
                return Conflict("Já existe oferta desse alvo com vigência iniciando nessa data.");
            }

            return null;
        }
    }
}
