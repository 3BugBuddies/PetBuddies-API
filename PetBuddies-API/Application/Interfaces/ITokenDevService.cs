using PetBuddies_API.Application.Dtos.Dev;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Interfaces
{
    public interface ITokenDevService
    {
        TokenDevDto Emitir(PerfilEnum perfil);
    }
}
