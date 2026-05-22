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
        public async Task<ActionResult<List<ClinicaDto>>> Listar()
        {
            return Ok(await _clinicaService.ListarAsync());
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca clínica por id")]
        public async Task<ActionResult<ClinicaDto>> BuscarPorId(int id)
        {
            var response = await _clinicaService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Clínica não encontrada para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra clínica")]
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
            return Created($"/api/clinica/{response.Id}", response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza clínica")]
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
