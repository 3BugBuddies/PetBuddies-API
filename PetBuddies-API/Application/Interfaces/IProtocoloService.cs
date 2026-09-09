using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IProtocoloService
    {
        Task<List<ProtocoloDto>> ListarAsync(
            EspecieEnum? especie = null,
            CategoriaProtocoloEnum? categoria = null,
            bool? ativo = null,
            CancellationToken cancellationToken = default);

        Task<ProtocoloDto?> BuscarPorIdAsync(long protocoloId, CancellationToken cancellationToken = default);

        /// <summary>
        /// As invariantes de domínio do protocolo. Devolve <c>null</c> quando o request é
        /// válido, e a mensagem da primeira violação quando não é.
        /// </summary>
        string? Validar(SalvarProtocoloRequest request);

        Task<ProtocoloDto> CadastrarAsync(SalvarProtocoloRequest request, CancellationToken cancellationToken = default);
        Task<ProtocoloDto?> AtualizarAsync(long protocoloId, SalvarProtocoloRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoverAsync(long protocoloId, CancellationToken cancellationToken = default);
    }
}
