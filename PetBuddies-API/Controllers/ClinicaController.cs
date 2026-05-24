using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.Clinica;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/clinica")]
    public class ClinicaController : ControllerBase
    {
        private readonly ClinicaService _clinicaService;

        public ClinicaController(ClinicaService clinicaService)
        {
            _clinicaService = clinicaService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista clínicas")]
        [SwaggerResponse(StatusCodes.Status200OK, "Clínicas listadas com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma clínica cadastrada.")]
        public async Task<ActionResult<List<ClinicaDto>>> Listar()
        {
            var response = await _clinicaService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("buscar")]
        [SwaggerOperation(Summary = "Busca clínicas por nome", Description = "Filtra clínicas cujo nome contenha o trecho informado (case-insensitive).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Clínicas encontradas.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhuma clínica encontrada.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Parâmetro 'nome' é obrigatório.")]
        public async Task<ActionResult<List<ClinicaDto>>> BuscarPorNome([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Parâmetro 'nome' é obrigatório.");

            var response = await _clinicaService.BuscarPorNomeAsync(nome);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca clínica por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Clínica encontrada.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Clínica não encontrada.")]
        public async Task<ActionResult<ClinicaDto>> BuscarPorId(int id)
        {
            var response = await _clinicaService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Clínica não encontrada para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra clínica")]
        [SwaggerResponse(StatusCodes.Status201Created, "Clínica cadastrada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar a clínica.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Endereço não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "CNPJ já cadastrado.")]
        public async Task<ActionResult<ClinicaDto>> Cadastrar([FromBody] SalvarClinicaRequest request)
        {
            if (!await _clinicaService.EnderecoExisteAsync(request.EnderecoId))
            {
                return NotFound("Endereço não encontrado para cadastrar clínica.");
            }

            if (await _clinicaService.CnpjExisteAsync(request.Cnpj))
            {
                return Conflict("Já existe uma clínica com este CNPJ.");
            }

            var response = await _clinicaService.CadastrarAsync(request);
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza clínica")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Clínica atualizada com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar a clínica.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Clínica ou endereço não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "CNPJ já cadastrado.")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarClinicaRequest request)
        {
            if (await _clinicaService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Clínica não encontrada para o id informado.");
            }

            if (!await _clinicaService.EnderecoExisteAsync(request.EnderecoId))
            {
                return NotFound("Endereço não encontrado para atualizar clínica.");
            }

            if (await _clinicaService.CnpjExisteAsync(request.Cnpj, id))
            {
                return Conflict("Já existe uma clínica com este CNPJ.");
            }

            await _clinicaService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove clínica")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Clínica removida com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Clínica não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Clínica possui vínculos e não pode ser removida.")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _clinicaService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Clínica não encontrada para o id informado.");
            }

            try
            {
                await _clinicaService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Clínica possui vínculos e não pode ser removida.");
            }
        }
    }
}
