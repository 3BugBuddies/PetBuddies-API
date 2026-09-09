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

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<RegraPontuacaoEntity?> ObterParaAlterarAsync(int regraId, CancellationToken cancellationToken = default);

        /// <summary>Um valor por clínica, gesto e vigência — o <c>UK_PONTUACAO_VIGENCIA</c>.</summary>
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
