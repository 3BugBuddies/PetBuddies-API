using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IOfertaRepository
    {
        Task<List<OfertaEntity>> ListarAsync(
            int? clinicaId = null,
            TipoAtoOfertaEnum? ato = null,
            CancellationToken cancellationToken = default);

        Task<OfertaEntity?> ObterPorIdAsync(int ofertaId, CancellationToken cancellationToken = default);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<OfertaEntity?> ObterParaAlterarAsync(int ofertaId, CancellationToken cancellationToken = default);

        /// <summary>
        /// A chave natural da vigência por sucessão: clínica, ato, alvo e data de início.
        /// Duas ofertas do mesmo alvo não podem começar a valer no mesmo dia.
        /// </summary>
        Task<bool> VigenciaExisteAsync(
            int clinicaId,
            TipoAtoOfertaEnum ato,
            string? subtipo,
            long? protocoloId,
            DateOnly inicioVigencia,
            int? ignorarOfertaId = null,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(OfertaEntity oferta, CancellationToken cancellationToken = default);
        void Remover(OfertaEntity oferta);
    }
}
