using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.Prontuario;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/prontuario")]
    public class ProntuarioController : ControllerBase
    {
        private readonly ProntuarioService _prontuarioService;

        public ProntuarioController(ProntuarioService prontuarioService)
        {
            _prontuarioService = prontuarioService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista prontuários")]
        public async Task<ActionResult<List<ProntuarioDto>>> Listar([FromQuery] int? animalId)
        {
            return Ok(await _prontuarioService.ListarAsync(animalId));
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca prontuário por id")]
        public async Task<ActionResult<ProntuarioDto>> BuscarPorId(int id)
        {
            var response = await _prontuarioService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Prontuário não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra prontuário")]
        public async Task<ActionResult<ProntuarioDto>> Cadastrar([FromBody] SalvarProntuarioRequest request)
        {
            if (!await _prontuarioService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para cadastrar prontuário.");
            }

            var response = await _prontuarioService.CadastrarAsync(request);
            return Created($"/api/prontuario/{response.Id}", response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza prontuário")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarProntuarioRequest request)
        {
            if (await _prontuarioService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Prontuário não encontrado para o id informado.");
            }

            if (!await _prontuarioService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para atualizar prontuário.");
            }

            await _prontuarioService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove prontuário")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _prontuarioService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Prontuário não encontrado para o id informado.");
            }

            try
            {
                await _prontuarioService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Prontuário possui vínculos e não pode ser removido.");
            }
        }
    }
}
