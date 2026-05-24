using Microsoft.AspNetCore.Mvc;
using PetBuddies_API.Dtos.Animal;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/animal")]
    public class AnimalController : ControllerBase
    {
        private readonly AnimalMotorService _animalMotorService;
        private readonly AnimalCadastroService _animalCadastroService;

        public AnimalController(
            AnimalMotorService animalMotorService,
            AnimalCadastroService animalCadastroService)
        {
            _animalMotorService = animalMotorService;
            _animalCadastroService = animalCadastroService;
        }

        [HttpGet("{id:int}/motor")]
        [SwaggerOperation(Summary = "Busca dados do animal para o motor Java")]
        [SwaggerResponse(StatusCodes.Status200OK, "Dados do animal encontrados.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal não encontrado.")]
        public async Task<ActionResult<AnimalMotorDto>> GetDadosMotor(int id)
        {
            var response = await _animalMotorService.GetDadosMotorAsync(id);

            if (response is null)
            {
                return NotFound("Animal não encontrado para o id informado.");
            }

            return Ok(response);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista animais")]
        [SwaggerResponse(StatusCodes.Status200OK, "Animais listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum animal cadastrado.")]
        public async Task<ActionResult<List<AnimalDto>>> Listar()
        {
            var response = await _animalCadastroService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca animal por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Animal encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal não encontrado.")]
        public async Task<ActionResult<AnimalDto>> BuscarPorId(int id)
        {
            var response = await _animalCadastroService.BuscarPorIdAsync(id);

            if (response is null)
            {
                return NotFound("Animal não encontrado para o id informado.");
            }

            return Ok(response);
        }

        [HttpGet("{id:int}/ultima-consulta")]
        [SwaggerOperation(Summary = "Busca a última consulta realizada do animal")]
        [SwaggerResponse(StatusCodes.Status200OK, "Última consulta encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhuma consulta realizada encontrada.")]
        public async Task<ActionResult<UltimaConsultaDto>> GetUltimaConsulta(int id)
        {
            var response = await _animalMotorService.GetUltimaConsultaAsync(id);

            if (response is null)
            {
                return NotFound("Nenhuma consulta realizada foi encontrada para o animal informado.");
            }

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra animal vinculado a um responsável")]
        [SwaggerResponse(StatusCodes.Status201Created, "Animal cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar o animal.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Responsável não encontrado.")]
        public async Task<ActionResult<AnimalDto>> Cadastrar([FromBody] CadastrarAnimalRequest request)
        {
            if (!await _animalCadastroService.ResponsavelExisteAsync(request.ResponsavelId))
            {
                return NotFound("Responsável não encontrado para vincular o animal.");
            }

            if (!await _animalCadastroService.TipoAnimalExisteAsync(request.Especie!.Value, request.Porte!.Value))
            {
                return BadRequest($"Não há TipoAnimal cadastrado para a combinação ({request.Especie}, {request.Porte}).");
            }

            var response = await _animalCadastroService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza animal")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Animal atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar o animal.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal ou responsável não encontrado.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAnimalRequest request)
        {
            if (!await _animalCadastroService.ExisteAsync(id))
            {
                return NotFound("Animal não encontrado para o id informado.");
            }

            if (!await _animalCadastroService.ResponsavelExisteAsync(request.ResponsavelId))
            {
                return NotFound("Responsável não encontrado para vincular o animal.");
            }

            if (!await _animalCadastroService.TipoAnimalExisteAsync(request.Especie!.Value, request.Porte!.Value))
            {
                return BadRequest($"Não há TipoAnimal cadastrado para a combinação ({request.Especie}, {request.Porte}).");
            }

            await _animalCadastroService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove animal")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Animal removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Animal não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Animal possui vínculos e não pode ser removido.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (!await _animalCadastroService.ExisteAsync(id))
            {
                return NotFound("Animal não encontrado para o id informado.");
            }

            if (await _animalCadastroService.PossuiConsultasAsync(id))
            {
                return Conflict("Animal possui consultas vinculadas e não pode ser removido.");
            }

            await _animalCadastroService.RemoverAsync(id);
            return NoContent();
        }
    }
}
