using PetBuddies_API.Application.Dtos.Veterinario;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IVeterinarioUseCase
    {
        Task<List<VeterinarioDto>> ListarAsync();
        Task<VeterinarioDto?> BuscarPorIdAsync(int veterinarioId);
        Task<List<VeterinarioDto>> ListarPorClinicaAsync(int clinicaId);
        Task<bool> ClinicaExisteAsync(int clinicaId);
        Task<bool> CrmvExisteAsync(string crmv, int? ignorarVeterinarioId = null);
        Task<VeterinarioDto> CadastrarAsync(SalvarVeterinarioRequest request);
        Task<VeterinarioDto?> AtualizarAsync(int veterinarioId, SalvarVeterinarioRequest request);
        Task<bool> RemoverAsync(int veterinarioId);
    }
}
