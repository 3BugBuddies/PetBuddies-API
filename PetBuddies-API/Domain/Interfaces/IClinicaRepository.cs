using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IClinicaRepository
    {
        Task<ClinicaEntity?> ObterPorIdAsync(int clinicaId);
        Task<ClinicaEntity?> ObterParaAlterarAsync(int clinicaId);
        Task<List<ClinicaEntity>> ListarAsync();
        Task<List<ClinicaEntity>> BuscarPorNomeAsync(string nome);
        Task<bool> CnpjExisteAsync(string cnpj, int? ignorarClinicaId = null);
        Task AdicionarAsync(ClinicaEntity clinica);
        void Remover(ClinicaEntity clinica);
    }
}
