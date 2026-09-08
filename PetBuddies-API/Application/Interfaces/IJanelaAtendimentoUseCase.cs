using PetBuddies_API.Application.Dtos.JanelaAtendimento;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IJanelaAtendimentoUseCase
    {
        Task<List<JanelaAtendimentoDto>> ListarDisponiveisAsync();
        Task<List<JanelaAtendimentoDto>> ListarAsync();
        Task<JanelaAtendimentoDto?> BuscarPorIdAsync(int janelaId);
        Task<bool> VeterinarioExisteAsync(int veterinarioId);
        Task<bool> HorarioExisteAsync(int veterinarioId, DateTime dataHoraInicio, int? ignorarJanelaId = null);
        Task<bool> PossuiConsultaAsync(int janelaId);
        Task<JanelaAtendimentoDto> CadastrarAsync(SalvarJanelaAtendimentoRequest request);
        Task<JanelaAtendimentoDto?> AtualizarAsync(int janelaId, SalvarJanelaAtendimentoRequest request);
        Task<bool> RemoverAsync(int janelaId);
    }
}
