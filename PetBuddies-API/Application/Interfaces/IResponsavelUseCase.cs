using PetBuddies_API.Application.Dtos.Animal;
using PetBuddies_API.Application.Dtos.Responsavel;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IResponsavelUseCase
    {
        Task<ResponsavelDto?> BuscarPorTelefoneAsync(string telefone);
        Task<List<ResponsavelDto>> ListarAsync();
        Task<ResponsavelDto?> BuscarPorIdAsync(int responsavelId);
        Task<bool> TelefoneExisteAsync(string telefone, int? ignorarResponsavelId = null);
        Task<bool> PossuiAnimaisAsync(int responsavelId);
        Task<ResponsavelDto> CadastrarAsync(CadastrarResponsavelRequest request);
        Task<ResponsavelDto?> AtualizarAsync(int responsavelId, CadastrarResponsavelRequest request);
        Task<bool> RemoverAsync(int responsavelId);
        Task<List<AnimalDto>?> ListarAnimaisAsync(int responsavelId);
    }
}
