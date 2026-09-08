using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IResponsavelRepository
    {
        Task<ResponsavelEntity?> ObterPorTelefoneAsync(string telefoneNormalizado);
        Task<List<ResponsavelEntity>> ListarAsync();
        Task<ResponsavelEntity?> ObterPorIdAsync(int responsavelId);
        Task<ResponsavelEntity?> ObterParaAlterarAsync(int responsavelId);
        Task<bool> TelefoneExisteAsync(string telefoneNormalizado, int? ignorarResponsavelId = null);
        Task<bool> PossuiAnimaisAsync(int responsavelId);
        Task<bool> ExisteAsync(int responsavelId);
        Task<List<AnimalEntity>> ListarAnimaisAsync(int responsavelId);
        Task AdicionarAsync(ResponsavelEntity responsavel);
        void Remover(ResponsavelEntity responsavel);
    }
}
