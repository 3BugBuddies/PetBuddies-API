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

        string? Validar(SalvarOfertaRequest request);

        Task<bool> ProtocoloExisteAsync(long protocoloId, CancellationToken cancellationToken = default);

        Task<bool> VigenciaExisteAsync(SalvarOfertaRequest request, int? ignorarOfertaId = null, CancellationToken cancellationToken = default);

        Task<OfertaDto> CadastrarAsync(SalvarOfertaRequest request, CancellationToken cancellationToken = default);
        Task<OfertaDto?> AtualizarAsync(int ofertaId, SalvarOfertaRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoverAsync(int ofertaId, CancellationToken cancellationToken = default);
    }
}
