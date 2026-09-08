using PetBuddies_API.Application.Dtos.Clinica;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class ClinicaMapper
    {
        public static ClinicaDto ToDto(this ClinicaEntity clinica)
        {
            return new ClinicaDto
            {
                Id = clinica.Id,
                Nome = clinica.Nome,
                Cnpj = clinica.Cnpj,
                Telefone = clinica.Telefone,
                Email = clinica.Email
            };
        }

        public static void Aplicar(this ClinicaEntity clinica, SalvarClinicaRequest request)
        {
            clinica.Nome = request.Nome.Trim();
            clinica.Cnpj = request.Cnpj.Trim();
            clinica.Telefone = request.Telefone.Trim();
            clinica.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        }
    }
}
