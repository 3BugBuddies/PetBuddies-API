using PetBuddies_API.Application.Dtos.Pontuacao;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IPontuacaoService
    {
        string? Validar(PontuarConsultaRequest request);

        Task<ResultadoPontuacaoDto> PontuarConsultaAsync(PontuarConsultaRequest request, CancellationToken cancellationToken = default);

        Task<SaldoPontosDto> ObterSaldoAsync(long responsavelId, CancellationToken cancellationToken = default);

        Task<List<LancamentoPontosDto>> ListarExtratoAsync(long responsavelId, CancellationToken cancellationToken = default);

        Task<MemoriaCalculoDto?> ObterMemoriaAsync(long consultaId, CancellationToken cancellationToken = default);
    }
}
