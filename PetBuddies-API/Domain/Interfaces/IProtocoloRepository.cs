using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IProtocoloRepository
    {
        Task<List<ProtocoloEntity>> ListarAsync(
            EspecieEnum? especie = null,
            CategoriaProtocoloEnum? categoria = null,
            bool? ativo = null,
            CancellationToken cancellationToken = default);

        Task<ProtocoloEntity?> ObterPorIdAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task<ProtocoloEntity?> ObterParaAlterarAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task<bool> ExisteAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(ProtocoloEntity protocolo, CancellationToken cancellationToken = default);
        Task RemoverAsync(ProtocoloEntity protocolo, CancellationToken cancellationToken = default);
        Task AtualizarAsync(ProtocoloEntity protocolo, CancellationToken cancellationToken = default);
    }
}
