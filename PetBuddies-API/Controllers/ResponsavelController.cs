using Microsoft.AspNetCore.Mvc;
using PetBuddies_API.Dtos.Animal;
using PetBuddies_API.Dtos.Responsavel;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/responsavel")]
    public class ResponsavelController : ControllerBase
    {
        private readonly ResponsavelService _responsavelService;

        public ResponsavelController(ResponsavelService responsavelService)
        {
            _responsavelService = responsavelService;
        }

        [HttpGet("buscar/{telefone}")]
        [SwaggerOperation(Summary = "Busca responsável por telefone")]
        [SwaggerResponse(StatusCodes.Status200OK, "Responsável encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Responsável não encontrado.")]
        public async Task<ActionResult<ResponsavelDto>> BuscarPorTelefone(string telefone)
        {
            var response = await _responsavelService.BuscarPorTelefoneAsync(telefone);

            if (response is null)
            {
                return NotFound("Responsável não encontrado para o telefone informado.");
            }

            return Ok(response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista responsáveis")]
        [SwaggerResponse(StatusCodes.Status200OK, "Responsáveis listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum responsável cadastrado.")]
        public async Task<ActionResult<List<ResponsavelDto>>> Listar()
        {
            var response = await _responsavelService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca responsável por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Responsável encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Responsável não encontrado.")]
        public async Task<ActionResult<ResponsavelDto>> BuscarPorId(int id)
        {
            var response = await _responsavelService.BuscarPorIdAsync(id);

            if (response is null)
            {
                return NotFound("Responsável não encontrado para o id informado.");
            }

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra responsável em pré-cadastro")]
        [SwaggerResponse(StatusCodes.Status201Created, "Responsável cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar o responsável.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Telefone já cadastrado.")]
        public async Task<ActionResult<ResponsavelDto>> Cadastrar([FromBody] CadastrarResponsavelRequest request)
        {
            if (await _responsavelService.TelefoneExisteAsync(request.Telefone))
            {
                return Conflict("Já existe um responsável cadastrado com este telefone.");
            }

            var response = await _responsavelService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza responsável")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Responsável atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar o responsável.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Responsável não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Telefone já cadastrado.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CadastrarResponsavelRequest request)
        {
            if (await _responsavelService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Responsável não encontrado para o id informado.");
            }

            if (await _responsavelService.TelefoneExisteAsync(request.Telefone, id))
            {
                return Conflict("Já existe um responsável cadastrado com este telefone.");
            }

            await _responsavelService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpGet("{id:int}/animal")]
        [SwaggerOperation(Summary = "Lista animais de um responsável")]
        [SwaggerResponse(StatusCodes.Status200OK, "Animais listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Responsável sem animais cadastrados.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Responsável não encontrado.")]
        public async Task<ActionResult<List<AnimalDto>>> ListarAnimais(int id)
        {
            var response = await _responsavelService.ListarAnimaisAsync(id);

            if (response is null)
            {
                return NotFound("Responsável não encontrado para o id informado.");
            }

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove responsável")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Responsável removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Responsável não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Responsável possui vínculos e não pode ser removido.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _responsavelService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Responsável não encontrado para o id informado.");
            }

            if (await _responsavelService.PossuiAnimaisAsync(id))
            {
                return Conflict("Responsável possui animais vinculados e não pode ser removido.");
            }

            await _responsavelService.RemoverAsync(id);
            return NoContent();
        }
    }
}
