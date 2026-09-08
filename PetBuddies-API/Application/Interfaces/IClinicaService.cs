using PetBuddies_API.Application.Dtos.Clinica;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IClinicaService
    {
        Task<List<ClinicaDto>> ListarAsync();
        Task<ClinicaDto?> BuscarPorIdAsync(int clinicaId);
        Task<List<ClinicaDto>> BuscarPorNomeAsync(string nome);
        Task<bool> CnpjExisteAsync(string cnpj, int? ignorarClinicaId = null);
        Task<ClinicaDto> CadastrarAsync(SalvarClinicaRequest request);
        Task<ClinicaDto?> AtualizarAsync(int clinicaId, SalvarClinicaRequest request);
        Task<bool> RemoverAsync(int clinicaId);
    }
}
