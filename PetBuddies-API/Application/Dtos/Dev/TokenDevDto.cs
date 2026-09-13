using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.Dev
{
    public record TokenDevDto
    {
        public string Token { get; init; } = string.Empty;
        public PerfilEnum Perfil { get; init; }
        public string Emissor { get; init; } = string.Empty;
        public DateTime ExpiraEm { get; init; }
    }
}
