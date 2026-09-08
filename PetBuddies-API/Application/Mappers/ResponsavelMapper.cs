using PetBuddies_API.Application.Dtos.Responsavel;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class ResponsavelMapper
    {
        public static ResponsavelDto ToDto(this ResponsavelEntity responsavel)
        {
            return new ResponsavelDto
            {
                Id = responsavel.Id,
                Nome = responsavel.Nome,
                Telefone = responsavel.Telefone,
                Email = responsavel.Email
            };
        }
    }
}
