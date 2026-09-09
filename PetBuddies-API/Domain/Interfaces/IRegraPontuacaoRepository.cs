using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IRegraPontuacaoRepository
    {
        Task<List<RegraPontuacaoEntity>> ListarAsync(
            int? clinicaId = null,
            TipoGestoEnum? gesto = null,
            CancellationToken cancellationToken = default);

        Task<RegraPontuacaoEntity?> ObterPorIdAsync(int regraId, CancellationToken cancellationToken = default);

        Task<RegraPontuacaoEntity?> ObterParaAlterarAsync(int regraId, CancellationToken cancellationToken = default);

        Task<bool> VigenciaExisteAsync(
            int clinicaId,
            TipoGestoEnum gesto,
            DateOnly inicioVigencia,
            int? ignorarRegraId = null,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(RegraPontuacaoEntity regra, CancellationToken cancellationToken = default);
        void Remover(RegraPontuacaoEntity regra);
    }
}
