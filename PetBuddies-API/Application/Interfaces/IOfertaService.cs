using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IOfertaService
    {
        Task<List<OfertaDto>> ListarAsync(
            int? clinicaId = null,
            TipoAtoOfertaEnum? ato = null,
            CancellationToken cancellationToken = default);

        Task<OfertaDto?> BuscarPorIdAsync(int ofertaId, CancellationToken cancellationToken = default);

        /// <summary>
        /// As invariantes de domínio da oferta: o alvo coerente com o ato
        /// (<c>CK_OFERTA_ALVO</c>) e o valor não negativo. Devolve <c>null</c> quando o request
        /// é válido.
        /// </summary>
        /// <remarks>
        /// Não valida sobreposição de vigência: a vigência é por sucessão, só tem data de
        /// início, e um preço vale até o próximo começar — não há intervalo para sobrepor.
        /// </remarks>
        string? Validar(SalvarOfertaRequest request);

        Task<bool> ProtocoloExisteAsync(long protocoloId, CancellationToken cancellationToken = default);

        /// <summary>Duas ofertas do mesmo alvo não podem começar a valer no mesmo dia.</summary>
        Task<bool> VigenciaExisteAsync(SalvarOfertaRequest request, int? ignorarOfertaId = null, CancellationToken cancellationToken = default);

        Task<OfertaDto> CadastrarAsync(SalvarOfertaRequest request, CancellationToken cancellationToken = default);
        Task<OfertaDto?> AtualizarAsync(int ofertaId, SalvarOfertaRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoverAsync(int ofertaId, CancellationToken cancellationToken = default);
    }
}
