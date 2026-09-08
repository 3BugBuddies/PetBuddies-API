using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IVeterinarioRepository
    {
        Task<List<VeterinarioEntity>> ListarAsync();
        Task<VeterinarioEntity?> ObterPorIdAsync(int veterinarioId);
        Task<VeterinarioEntity?> ObterParaAlterarAsync(int veterinarioId);
        Task<List<VeterinarioEntity>> ListarPorClinicaAsync(int clinicaId);
        Task<bool> ClinicaExisteAsync(int clinicaId);
        Task<bool> CrmvExisteAsync(string crmv, int? ignorarVeterinarioId = null);
        Task AdicionarAsync(VeterinarioEntity veterinario);
        void Remover(VeterinarioEntity veterinario);
    }
}
