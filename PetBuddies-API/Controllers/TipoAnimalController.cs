using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.TipoAnimal;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/tipo-animal")]
    public class TipoAnimalController : ControllerBase
    {
        private readonly TipoAnimalService _tipoAnimalService;

        public TipoAnimalController(TipoAnimalService tipoAnimalService)
        {
            _tipoAnimalService = tipoAnimalService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista tipos de animal")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tipos de animal listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum tipo de animal cadastrado.")]
        public async Task<ActionResult<List<TipoAnimalDto>>> Listar()
        {
            var response = await _tipoAnimalService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca tipo de animal por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tipo de animal encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tipo de animal não encontrado.")]
        public async Task<ActionResult<TipoAnimalDto>> BuscarPorId(int id)
        {
            var response = await _tipoAnimalService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Tipo de animal não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra tipo de animal")]
        [SwaggerResponse(StatusCodes.Status201Created, "Tipo de animal cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar o tipo de animal.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Combinação de espécie, porte e raça já cadastrada.")]
        public async Task<ActionResult<TipoAnimalDto>> Cadastrar([FromBody] SalvarTipoAnimalRequest request)
        {
            if (await _tipoAnimalService.DuplicadoAsync(request))
            {
                return Conflict("Já existe tipo de animal com espécie, porte e raça informados.");
            }

            var response = await _tipoAnimalService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza tipo de animal")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Tipo de animal atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar o tipo de animal.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tipo de animal não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Combinação de espécie, porte e raça já cadastrada.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarTipoAnimalRequest request)
        {
            if (await _tipoAnimalService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Tipo de animal não encontrado para o id informado.");
            }

            if (await _tipoAnimalService.DuplicadoAsync(request, id))
            {
                return Conflict("Já existe tipo de animal com espécie, porte e raça informados.");
            }

            await _tipoAnimalService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove tipo de animal")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Tipo de animal removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tipo de animal não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Tipo de animal possui vínculos e não pode ser removido.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _tipoAnimalService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Tipo de animal não encontrado para o id informado.");
            }

            try
            {
                await _tipoAnimalService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Tipo de animal possui vínculos e não pode ser removido.");
            }
        }
    }
}
