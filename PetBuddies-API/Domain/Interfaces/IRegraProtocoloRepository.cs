using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IRegraProtocoloRepository
    {
        Task<List<RegraProtocoloEntity>> ListarPorProtocoloAsync(
            long protocoloId,
            CancellationToken cancellationToken = default);

        Task<RegraProtocoloEntity?> ObterPorIdAsync(long regraId, CancellationToken cancellationToken = default);

        Task<RegraProtocoloEntity?> ObterParaAlterarAsync(long regraId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(RegraProtocoloEntity regra, CancellationToken cancellationToken = default);
        Task RemoverAsync(RegraProtocoloEntity regra, CancellationToken cancellationToken = default);
        Task AtualizarAsync(RegraProtocoloEntity regra, CancellationToken cancellationToken = default);
    }
}
