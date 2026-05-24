using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.Endereco;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/endereco")]
    public class EnderecoController : ControllerBase
    {
        private readonly EnderecoService _enderecoService;

        public EnderecoController(EnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista endereços")]
        [SwaggerResponse(StatusCodes.Status200OK, "Endereços listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum endereço cadastrado.")]
        public async Task<ActionResult<List<EnderecoDto>>> Listar()
        {
            var response = await _enderecoService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca endereço por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Endereço encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Endereço não encontrado.")]
        public async Task<ActionResult<EnderecoDto>> BuscarPorId(int id)
        {
            var response = await _enderecoService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Endereço não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra endereço")]
        [SwaggerResponse(StatusCodes.Status201Created, "Endereço cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar o endereço.")]
        public async Task<ActionResult<EnderecoDto>> Cadastrar([FromBody] SalvarEnderecoRequest request)
        {
            var response = await _enderecoService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza endereço")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Endereço atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar o endereço.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Endereço não encontrado.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarEnderecoRequest request)
        {
            if (!await _enderecoService.ExisteAsync(id))
            {
                return NotFound("Endereço não encontrado para o id informado.");
            }

            await _enderecoService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove endereço")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Endereço removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Endereço não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Endereço possui vínculos e não pode ser removido.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (!await _enderecoService.ExisteAsync(id))
            {
                return NotFound("Endereço não encontrado para o id informado.");
            }

            try
            {
                await _enderecoService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Endereço possui vínculos e não pode ser removido.");
            }
        }
    }
}
