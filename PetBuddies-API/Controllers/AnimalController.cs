using Microsoft.AspNetCore.Mvc;
using PetBuddies_API.Dtos.Client;
using PetBuddies_API.Services;

namespace PetBuddies_API.Controllers
{
    [ApiController]
    [Route("api/animal")]
    public class AnimalController : ControllerBase
    {
        private readonly AnimalMotorService _animalMotorService;

        public AnimalController(AnimalMotorService animalMotorService)
        {
            _animalMotorService = animalMotorService;
        }

        [HttpGet("{id:int}/motor")]
        [ProducesResponseType(typeof(AnimalMotorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AnimalMotorDto>> GetDadosMotor(int id, CancellationToken cancellationToken)
        {
            var response = await _animalMotorService.GetDadosMotorAsync(id, cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("{id:int}/ultima-consulta")]
        [ProducesResponseType(typeof(UltimaConsultaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UltimaConsultaDto>> GetUltimaConsulta(int id, CancellationToken cancellationToken)
        {
            var response = await _animalMotorService.GetUltimaConsultaAsync(id, cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
