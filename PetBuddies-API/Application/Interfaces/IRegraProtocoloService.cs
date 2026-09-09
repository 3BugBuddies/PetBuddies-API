using PetBuddies_API.Application.Dtos.RegraProtocolo;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IRegraProtocoloService
    {
        Task<List<RegraProtocoloDto>> ListarPorProtocoloAsync(long protocoloId, CancellationToken cancellationToken = default);
        Task<RegraProtocoloDto?> BuscarPorIdAsync(long regraId, CancellationToken cancellationToken = default);

        string? Validar(SalvarRegraProtocoloRequest request);

        Task<bool> ProtocoloExisteAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task<RegraProtocoloDto> CadastrarAsync(SalvarRegraProtocoloRequest request, CancellationToken cancellationToken = default);
        Task<RegraProtocoloDto?> AtualizarAsync(long regraId, SalvarRegraProtocoloRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoverAsync(long regraId, CancellationToken cancellationToken = default);
    }
}
