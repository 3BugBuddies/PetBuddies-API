using PetBuddies_API.Application.Dtos.Consulta;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class ConsultaMapper
    {
        public static ConsultaDto ToDto(this ConsultaEntity consulta)
        {
            return new ConsultaDto
            {
                Id = consulta.Id,
                TipoConsulta = consulta.TipoConsulta.ToString(),
                DataHora = consulta.DataHora,
                Status = consulta.Status.ToString(),
                AnimalId = consulta.AnimalId
            };
        }
    }
}
