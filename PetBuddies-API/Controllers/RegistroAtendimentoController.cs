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
        public async Task<ActionResult<List<RegistroAtendimentoDto>>> Listar([FromQuery] int? animalId)
        {
            return Ok(await _registroAtendimentoService.ListarAsync(animalId));
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca registro de atendimento por id")]
        public async Task<ActionResult<RegistroAtendimentoDto>> BuscarPorId(int id)
        {
            var response = await _registroAtendimentoService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Registro de atendimento não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra registro de atendimento")]
        public async Task<ActionResult<RegistroAtendimentoDto>> Cadastrar([FromBody] SalvarRegistroAtendimentoRequest request)
        {
            var validacao = await ValidarReferenciasAsync(request);
            if (validacao is not null)
            {
                return validacao;
            }

            var response = await _registroAtendimentoService.CadastrarAsync(request);
            return Created($"/api/registro-atendimento/{response.Id}", response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza registro de atendimento")]
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
