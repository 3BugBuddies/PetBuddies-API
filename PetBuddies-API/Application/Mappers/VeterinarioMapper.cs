using PetBuddies_API.Application.Dtos.Veterinario;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class VeterinarioMapper
    {
        public static VeterinarioDto ToDto(this VeterinarioEntity veterinario)
        {
            return new VeterinarioDto
            {
                Id = veterinario.Id,
                Nome = veterinario.Nome,
                Crmv = veterinario.Crmv,
                Email = veterinario.Email,
                Ativo = veterinario.Ativo,
                ClinicaId = veterinario.ClinicaId
            };
        }

        public static void Aplicar(this VeterinarioEntity veterinario, SalvarVeterinarioRequest request)
        {
            veterinario.Nome = request.Nome.Trim();
            veterinario.Crmv = request.Crmv.Trim();
            veterinario.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            veterinario.Ativo = request.Ativo;
            veterinario.ClinicaId = request.ClinicaId;
        }
    }
}
