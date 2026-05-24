using Microsoft.AspNetCore.Mvc;
using PetBuddies_API.Dtos.JanelaAtendimento;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/janela-atendimento")]
    public class JanelaAtendimentoController : ControllerBase
    {
        private readonly JanelaAtendimentoService _janelaAtendimentoService;

        public JanelaAtendimentoController(JanelaAtendimentoService janelaAtendimentoService)
        {
            _janelaAtendimentoService = janelaAtendimentoService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista janelas de atendimento disponíveis")]
        [SwaggerResponse(StatusCodes.Status200OK, "Janelas de atendimento listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma janela de atendimento disponível.")]
        public async Task<ActionResult<List<JanelaAtendimentoDto>>> ListarDisponiveis()
        {
            var response = await _janelaAtendimentoService.ListarDisponiveisAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("todas")]
        [SwaggerOperation(Summary = "Lista todas as janelas de atendimento")]
        [SwaggerResponse(StatusCodes.Status200OK, "Janelas de atendimento listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma janela de atendimento cadastrada.")]
        public async Task<ActionResult<List<JanelaAtendimentoDto>>> Listar()
        {
            var response = await _janelaAtendimentoService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca janela de atendimento por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Janela de atendimento encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Janela de atendimento não encontrada.")]
        public async Task<ActionResult<JanelaAtendimentoDto>> BuscarPorId(int id)
        {
            var response = await _janelaAtendimentoService.BuscarPorIdAsync(id);

            if (response is null)
            {
                return NotFound("Janela de atendimento não encontrada para o id informado.");
            }

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra janela de atendimento")]
        [SwaggerResponse(StatusCodes.Status201Created, "Janela de atendimento cadastrada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar a janela.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Veterinário não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Horário já cadastrado.")]
        public async Task<ActionResult<JanelaAtendimentoDto>> Cadastrar([FromBody] SalvarJanelaAtendimentoRequest request)
        {
            if (request.DataHoraFim <= request.DataHoraInicio)
            {
                return BadRequest("DataHoraFim deve ser maior que DataHoraInicio.");
            }

            if (!await _janelaAtendimentoService.VeterinarioExisteAsync(request.VeterinarioId))
            {
                return NotFound("Veterinário não encontrado para cadastrar janela de atendimento.");
            }

            if (await _janelaAtendimentoService.HorarioExisteAsync(
                request.VeterinarioId,
                request.DataHoraInicio!.Value))
            {
                return Conflict("Já existe janela de atendimento neste horário para o veterinário.");
            }

            var response = await _janelaAtendimentoService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza janela de atendimento")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Janela de atendimento atualizada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar a janela.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Janela ou veterinário não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Janela possui conflito de horário ou vínculo.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarJanelaAtendimentoRequest request)
        {
            if (request.DataHoraFim <= request.DataHoraInicio)
            {
                return BadRequest("DataHoraFim deve ser maior que DataHoraInicio.");
            }

            if (await _janelaAtendimentoService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Janela de atendimento não encontrada para o id informado.");
            }

            if (!await _janelaAtendimentoService.VeterinarioExisteAsync(request.VeterinarioId))
            {
                return NotFound("Veterinário não encontrado para atualizar janela de atendimento.");
            }

            if (await _janelaAtendimentoService.PossuiConsultaAsync(id))
            {
                return Conflict("Janela de atendimento possui consulta vinculada e não pode ser alterada.");
            }

            if (await _janelaAtendimentoService.HorarioExisteAsync(
                request.VeterinarioId,
                request.DataHoraInicio!.Value,
                id))
            {
                return Conflict("Já existe janela de atendimento neste horário para o veterinário.");
            }

            await _janelaAtendimentoService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove janela de atendimento")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Janela de atendimento removida com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Janela de atendimento não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Janela possui vínculos e não pode ser removida.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _janelaAtendimentoService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Janela de atendimento não encontrada para o id informado.");
            }

            if (await _janelaAtendimentoService.PossuiConsultaAsync(id))
            {
                return Conflict("Janela de atendimento possui consulta vinculada e não pode ser removida.");
            }

            await _janelaAtendimentoService.RemoverAsync(id);
            return NoContent();
        }
    }
}
