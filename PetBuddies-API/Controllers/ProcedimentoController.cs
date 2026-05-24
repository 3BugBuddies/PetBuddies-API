using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.Procedimento;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/procedimento")]
    public class ProcedimentoController : ControllerBase
    {
        private readonly ProcedimentoService _procedimentoService;

        public ProcedimentoController(ProcedimentoService procedimentoService)
        {
            _procedimentoService = procedimentoService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista procedimentos")]
        [SwaggerResponse(StatusCodes.Status200OK, "Procedimentos listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum procedimento encontrado.")]
        public async Task<ActionResult<List<ProcedimentoDto>>> Listar([FromQuery] int? animalId)
        {
            var response = await _procedimentoService.ListarAsync(animalId);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca procedimento por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Procedimento encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Procedimento não encontrado.")]
        public async Task<ActionResult<ProcedimentoDto>> BuscarPorId(int id)
        {
            var response = await _procedimentoService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Procedimento não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra procedimento")]
        [SwaggerResponse(StatusCodes.Status201Created, "Procedimento cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos ou referências não encontradas.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal, veterinário ou registro de atendimento não encontrado.")]
        public async Task<ActionResult<ProcedimentoDto>> Cadastrar([FromBody] SalvarProcedimentoRequest request)
        {
            var validacao = await ValidarReferenciasAsync(request);
            if (validacao is not null)
            {
                return validacao;
            }

            var response = await _procedimentoService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza procedimento")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Procedimento atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos ou referências não encontradas.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Procedimento, animal, veterinário ou registro não encontrado.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarProcedimentoRequest request)
        {
            if (await _procedimentoService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Procedimento não encontrado para o id informado.");
            }

            var validacao = await ValidarReferenciasAsync(request);
            if (validacao is not null)
            {
                return validacao;
            }

            await _procedimentoService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove procedimento")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Procedimento removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Procedimento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Procedimento possui vínculos e não pode ser removido.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _procedimentoService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Procedimento não encontrado para o id informado.");
            }

            try
            {
                await _procedimentoService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Procedimento possui vínculos e não pode ser removido.");
            }
        }

        private async Task<ActionResult?> ValidarReferenciasAsync(SalvarProcedimentoRequest request)
        {
            if (!await _procedimentoService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para cadastrar procedimento.");
            }

            if (!await _procedimentoService.VeterinarioExisteAsync(request.VeterinarioId))
            {
                return NotFound("Veterinário não encontrado para cadastrar procedimento.");
            }

            if (!await _procedimentoService.RegistroAtendimentoPertenceAoAnimalAsync(
                request.RegistroAtendimentoId,
                request.AnimalId))
            {
                return BadRequest("Registro de atendimento não encontrado ou não pertence ao animal.");
            }

            return null;
        }
    }
}
