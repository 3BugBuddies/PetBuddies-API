using PetBuddies_API.Application.Dtos.Procedimento;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IProcedimentoService
    {
        Task<List<ProcedimentoDto>> ListarAsync(int? animalId = null);
        Task<ProcedimentoDto?> BuscarPorIdAsync(int procedimentoId);
        Task<bool> AnimalExisteAsync(int animalId);
        Task<bool> VeterinarioExisteAsync(int veterinarioId);
        Task<bool> RegistroAtendimentoPertenceAoAnimalAsync(int registroAtendimentoId, int animalId);
        Task<ProcedimentoDto> CadastrarAsync(SalvarProcedimentoRequest request);
        Task<ProcedimentoDto?> AtualizarAsync(int procedimentoId, SalvarProcedimentoRequest request);
        Task<bool> RemoverAsync(int procedimentoId);
    }
}
