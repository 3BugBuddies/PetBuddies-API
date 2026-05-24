using Microsoft.AspNetCore.Mvc;
using PetBuddies_API.Dtos.Consulta;
using PetBuddies_API.Enums;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/consulta")]
    public class ConsultaController : ControllerBase
    {
        private readonly ConsultaService _consultaService;

        public ConsultaController(ConsultaService consultaService)
        {
            _consultaService = consultaService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Agenda consulta em uma janela disponível")]
        [SwaggerResponse(StatusCodes.Status201Created, "Consulta agendada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para agendar a consulta.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal ou janela de atendimento não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Janela ocupada.")]
        public async Task<ActionResult<ConsultaDto>> Agendar([FromBody] AgendarConsultaRequest request)
        {
            if (!await _consultaService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para agendar consulta.");
            }

            if (!await _consultaService.JanelaExisteAsync(request.JanelaId))
            {
                return NotFound("Janela de atendimento não encontrada.");
            }

            if (await _consultaService.JanelaOcupadaAsync(request.JanelaId))
            {
                return Conflict("A janela de atendimento selecionada já está ocupada.");
            }

            var response = await _consultaService.AgendarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista consultas")]
        [SwaggerResponse(StatusCodes.Status200OK, "Consultas listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma consulta encontrada.")]
        public async Task<ActionResult<List<ConsultaDto>>> Listar([FromQuery] int? animalId)
        {
            var response = animalId.HasValue
                ? await _consultaService.ListarPorAnimalAsync(animalId.Value)
                : await _consultaService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca consulta por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Consulta encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Consulta não encontrada.")]
        public async Task<ActionResult<ConsultaDto>> BuscarPorId(int id)
        {
            var response = await _consultaService.BuscarPorIdAsync(id);

            if (response is null)
            {
                return NotFound("Consulta não encontrada para o id informado.");
            }

            return Ok(response);
        }

        [HttpPatch("{id:int}")]
        [SwaggerOperation(Summary = "Cancela uma consulta")]
        [SwaggerResponse(StatusCodes.Status200OK, "Consulta cancelada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cancelar a consulta.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Consulta não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Consulta já realizada.")]
        public async Task<ActionResult<ConsultaDto>> Cancelar(
            int id,
            [FromBody] AtualizarConsultaStatusRequest request)
        {
            if (request.Status != StatusConsultaEnum.CANCELADA)
            {
                return BadRequest("Nesta sprint, o PATCH de consulta aceita apenas o status CANCELADA.");
            }

            if (await _consultaService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Consulta não encontrada para o id informado.");
            }

            if (await _consultaService.ConsultaRealizadaAsync(id))
            {
                return Conflict("Consulta já realizada não pode ser cancelada.");
            }

            var response = await _consultaService.CancelarAsync(id, request);
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza consulta")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Consulta atualizada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar a consulta.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Consulta, animal ou janela não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Janela ocupada.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarConsultaRequest request)
        {
            if (await _consultaService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Consulta não encontrada para o id informado.");
            }

            if (!await _consultaService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para atualizar consulta.");
            }

            if (!await _consultaService.JanelaExisteAsync(request.JanelaId))
            {
                return NotFound("Janela de atendimento não encontrada.");
            }

            if (await _consultaService.JanelaOcupadaAsync(request.JanelaId, id))
            {
                return Conflict("A janela de atendimento selecionada já está ocupada.");
            }

            await _consultaService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove consulta")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Consulta removida com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Consulta não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Consulta não pode ser removida.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _consultaService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Consulta não encontrada para o id informado.");
            }

            if (await _consultaService.ConsultaRealizadaAsync(id))
            {
                return Conflict("Consulta já realizada não pode ser removida.");
            }

            await _consultaService.RemoverAsync(id);
            return NoContent();
        }
    }
}
