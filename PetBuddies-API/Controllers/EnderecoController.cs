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
        public async Task<ActionResult<List<EnderecoDto>>> Listar()
        {
            return Ok(await _enderecoService.ListarAsync());
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca endereço por id")]
        public async Task<ActionResult<EnderecoDto>> BuscarPorId(int id)
        {
            var response = await _enderecoService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Endereço não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra endereço")]
        public async Task<ActionResult<EnderecoDto>> Cadastrar([FromBody] SalvarEnderecoRequest request)
        {
            var response = await _enderecoService.CadastrarAsync(request);
            return Created($"/api/endereco/{response.Id}", response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza endereço")]
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
