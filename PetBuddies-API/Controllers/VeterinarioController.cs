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
        [SwaggerResponse(StatusCodes.Status200OK, "Veterinários listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum veterinário cadastrado.")]
        public async Task<ActionResult<List<VeterinarioDto>>> Listar()
        {
            var response = await _veterinarioService.ListarAsync();

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("por-clinica/{clinicaId:int}")]
        [SwaggerOperation(Summary = "Lista veterinários por clínica", Description = "Retorna todos os veterinários vinculados à clínica informada, ordenados por nome.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Veterinários da clínica listados com sucesso.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum veterinário encontrado para a clínica.")]
        public async Task<ActionResult<List<VeterinarioDto>>> ListarPorClinica(int clinicaId)
        {
            var response = await _veterinarioService.ListarPorClinicaAsync(clinicaId);

            if (response.Count == 0)
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca veterinário por id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Veterinário encontrado.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Veterinário não encontrado.")]
        public async Task<ActionResult<VeterinarioDto>> BuscarPorId(int id)
        {
            var response = await _veterinarioService.BuscarPorIdAsync(id);
            return response is null
                ? NotFound("Veterinário não encontrado para o id informado.")
                : Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra veterinário")]
        [SwaggerResponse(StatusCodes.Status201Created, "Veterinário cadastrado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para cadastrar o veterinário.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Clínica não encontrada.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "CRMV já cadastrado na clínica.")]
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
            return CreatedAtAction(nameof(BuscarPorId), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza veterinário")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Veterinário atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados inválidos para atualizar o veterinário.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Veterinário ou clínica não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "CRMV já cadastrado na clínica.")]
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
        [SwaggerResponse(StatusCodes.Status204NoContent, "Veterinário removido com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Veterinário não encontrado.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Veterinário possui vínculos e não pode ser removido.")]
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
