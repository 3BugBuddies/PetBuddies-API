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

        Task<OfertaEntity?> ObterParaAlterarAsync(int ofertaId, CancellationToken cancellationToken = default);

        Task<bool> VigenciaExisteAsync(
            int clinicaId,
            TipoAtoOfertaEnum ato,
            string? subtipo,
            long? protocoloId,
            DateOnly inicioVigencia,
            int? ignorarOfertaId = null,
            CancellationToken cancellationToken = default);

        Task AdicionarAsync(OfertaEntity oferta, CancellationToken cancellationToken = default);
        Task RemoverAsync(OfertaEntity oferta, CancellationToken cancellationToken = default);
        Task AtualizarAsync(OfertaEntity oferta, CancellationToken cancellationToken = default);
    }
}
