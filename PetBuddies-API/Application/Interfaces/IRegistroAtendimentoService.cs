using PetBuddies_API.Application.Dtos.RegistroAtendimento;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IRegistroAtendimentoService
    {
        Task<List<RegistroAtendimentoDto>> ListarAsync(int? animalId = null);
        Task<RegistroAtendimentoDto?> BuscarPorIdAsync(int registroAtendimentoId);
        Task<bool> AnimalExisteAsync(int animalId);
        Task<bool> ConsultaPertenceAoAnimalAsync(int consultaId, int animalId);
        Task<RegistroAtendimentoDto> CadastrarAsync(SalvarRegistroAtendimentoRequest request);
        Task<RegistroAtendimentoDto?> AtualizarAsync(int registroAtendimentoId, SalvarRegistroAtendimentoRequest request);
        Task<bool> RemoverAsync(int registroAtendimentoId);
    }
}
