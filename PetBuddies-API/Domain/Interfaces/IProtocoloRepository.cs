using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IProtocoloRepository
    {
        /// <summary>
        /// O catálogo que o motor do Java lê ao criar um plano: os protocolos vêm com as
        /// regras dentro, porque é delas que os itens são materializados.
        /// </summary>
        Task<List<ProtocoloEntity>> ListarAsync(
            EspecieEnum? especie = null,
            CategoriaProtocoloEnum? categoria = null,
            bool? ativo = null,
            CancellationToken cancellationToken = default);

        Task<ProtocoloEntity?> ObterPorIdAsync(long protocoloId, CancellationToken cancellationToken = default);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<ProtocoloEntity?> ObterParaAlterarAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task<bool> ExisteAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(ProtocoloEntity protocolo, CancellationToken cancellationToken = default);
        void Remover(ProtocoloEntity protocolo);
    }
}
