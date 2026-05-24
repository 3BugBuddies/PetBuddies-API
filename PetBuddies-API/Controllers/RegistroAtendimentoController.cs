using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.RegistroAtendimento;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/registro-atendimento")]
    public class RegistroAtendimentoController : ControllerBase
    {
        private readonly RegistroAtendimentoService _registroAtendimentoService;

        public RegistroAtendimentoController(RegistroAtendimentoService registroAtendimentoService)
        {
            _registroAtendimentoService = registroAtendimentoService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista registros de atendimento")]
        [SwaggerResponse(StatusCodes.Status200OK, "Registros de atendimento listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum registro de atendimento encontrado.")]
        public async Task<ActionResult<List<RegistroAtendimentoDto>>> Listar([FromQuery] int? animalId)
        {
            var response = await _registroAtendimentoService.ListarAsync(animalId);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca registro de atendimento por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Registro de atendimento encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Registro de atendimento não encontrado.")]
        public async Task<ActionResult<RegistroAtendimentoDto>> BuscarPorId(int id)
        {
            var response = await _registroAtendimentoService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Registro de atendimento não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra registro de atendimento")]
        [SwaggerResponse(StatusCodes.Status201Created, "Registro de atendimento cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos ou referências inconsistentes.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal, consulta ou prontuário não encontrado.")]
        public async Task<ActionResult<RegistroAtendimentoDto>> Cadastrar([FromBody] SalvarRegistroAtendimentoRequest request)
        {
            var validacao = await ValidarReferenciasAsync(request);
            if (validacao is not null)
            {
                return validacao;
            }

            var response = await _registroAtendimentoService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza registro de atendimento")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Registro de atendimento atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos ou referências inconsistentes.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Registro, animal, consulta ou prontuário não encontrado.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarRegistroAtendimentoRequest request)
        {
            if (await _registroAtendimentoService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Registro de atendimento não encontrado para o id informado.");
            }

            var validacao = await ValidarReferenciasAsync(request);
            if (validacao is not null)
            {
                return validacao;
            }

            await _registroAtendimentoService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove registro de atendimento")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Registro de atendimento removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Registro de atendimento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Registro de atendimento possui vínculos e não pode ser removido.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _registroAtendimentoService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Registro de atendimento não encontrado para o id informado.");
            }

            try
            {
                await _registroAtendimentoService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Registro de atendimento possui vínculos e não pode ser removido.");
            }
        }

        private async Task<ActionResult?> ValidarReferenciasAsync(SalvarRegistroAtendimentoRequest request)
        {
            if (!await _registroAtendimentoService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para registrar atendimento.");
            }

            if (!await _registroAtendimentoService.ConsultaPertenceAoAnimalAsync(request.ConsultaId, request.AnimalId))
            {
                return BadRequest("Consulta não encontrada ou não pertence ao animal.");
            }

            if (request.ProntuarioId.HasValue
                && !await _registroAtendimentoService.ProntuarioPertenceAoAnimalAsync(request.ProntuarioId.Value, request.AnimalId))
            {
                return BadRequest("Prontuário informado não pertence ao animal.");
            }

            return null;
        }
    }
}
