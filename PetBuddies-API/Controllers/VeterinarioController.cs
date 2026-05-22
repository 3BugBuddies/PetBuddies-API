using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Dtos.Veterinario;
using PetBuddies_API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/veterinario")]
    public class VeterinarioController : ControllerBase
    {
        private readonly VeterinarioService _veterinarioService;

        public VeterinarioController(VeterinarioService veterinarioService)
        {
            _veterinarioService = veterinarioService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista veterinários")]
        public async Task<ActionResult<List<VeterinarioDto>>> Listar()
        {
            return Ok(await _veterinarioService.ListarAsync());
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca veterinário por id")]
        public async Task<ActionResult<VeterinarioDto>> BuscarPorId(int id)
        {
            var response = await _veterinarioService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Veterinário não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra veterinário")]
        public async Task<ActionResult<VeterinarioDto>> Cadastrar([FromBody] SalvarVeterinarioRequest request)
        {
            if (!await _veterinarioService.ClinicaExisteAsync(request.ClinicaId))
            {
                return NotFound("Clínica não encontrada para cadastrar veterinário.");
            }

            if (await _veterinarioService.CrmvExisteAsync(request.Crmv, request.ClinicaId))
            {
                return Conflict("Já existe veterinário com este CRMV na clínica informada.");
            }

            var response = await _veterinarioService.CadastrarAsync(request);
            return Created($"/api/veterinario/{response.Id}", response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza veterinário")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarVeterinarioRequest request)
        {
            if (await _veterinarioService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Veterinário não encontrado para o id informado.");
            }

            if (!await _veterinarioService.ClinicaExisteAsync(request.ClinicaId))
            {
                return NotFound("Clínica não encontrada para atualizar veterinário.");
            }

            if (await _veterinarioService.CrmvExisteAsync(request.Crmv, request.ClinicaId, id))
            {
                return Conflict("Já existe veterinário com este CRMV na clínica informada.");
            }

            await _veterinarioService.AtualizarAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Remove veterinário")]
        public async Task<IActionResult> Remover(int id)
        {
            if (await _veterinarioService.BuscarPorIdAsync(id) is null)
            {
                return NotFound("Veterinário não encontrado para o id informado.");
            }

            try
            {
                await _veterinarioService.RemoverAsync(id);
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Veterinário possui vínculos e não pode ser removido.");
            }
        }
    }
}
