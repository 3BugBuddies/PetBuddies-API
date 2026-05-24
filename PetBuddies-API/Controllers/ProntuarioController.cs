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
        [SwaggerResponse(StatusCodes.Status200OK, "Prontuários listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum prontuário encontrado.")]
        public async Task<ActionResult<List<ProntuarioDto>>> Listar([FromQuery] int? animalId)
        {
            var response = await _prontuarioService.ListarAsync(animalId);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca prontuário por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Prontuário encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prontuário não encontrado.")]
        public async Task<ActionResult<ProntuarioDto>> BuscarPorId(int id)
        {
            var response = await _prontuarioService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Prontuário não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra prontuário")]
        [SwaggerResponse(StatusCodes.Status201Created, "Prontuário cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar o prontuário.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal não encontrado.")]
        public async Task<ActionResult<ProntuarioDto>> Cadastrar([FromBody] SalvarProntuarioRequest request)
        {
            if (!await _prontuarioService.AnimalExisteAsync(request.AnimalId))
            {
                return NotFound("Animal não encontrado para cadastrar prontuário.");
            }

            var response = await _prontuarioService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza prontuário")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prontuário atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar o prontuário.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prontuário ou animal não encontrado.")]
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prontuário removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prontuário não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Prontuário possui vínculos e não pode ser removido.")]
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
