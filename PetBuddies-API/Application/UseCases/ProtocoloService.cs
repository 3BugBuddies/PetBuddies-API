using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class ProtocoloService : IProtocoloService
    {
        private readonly IProtocoloRepository _repositorio;

        public ProtocoloService(IProtocoloRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<ProtocoloDto>> ListarAsync(
            EspecieEnum? especie = null,
            CategoriaProtocoloEnum? categoria = null,
            bool? ativo = null,
            CancellationToken cancellationToken = default)
        {
            var protocolos = await _repositorio.ListarAsync(especie, categoria, ativo, cancellationToken);
            return protocolos.Select(protocolo => protocolo.ToDto()).ToList();
        }

        public async Task<ProtocoloDto?> BuscarPorIdAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            var protocolo = await _repositorio.ObterPorIdAsync(protocoloId, cancellationToken);
            return protocolo?.ToDto();
        }

        // Espelha CK_PROTOCOLO_CATEGORIA e CK_PROTOCOLO_ESPECIE.
        public string? Validar(SalvarProtocoloRequest request)
        {
            if (request.Categoria is null)
            {
                return "Categoria do protocolo é obrigatória.";
            }

            if (!Enum.IsDefined(request.Categoria.Value))
            {
                return "Categoria do protocolo deve ser PREVENTIVO ou POS_CIRURGICO.";
            }

            if (request.Especie is null || !Enum.IsDefined(request.Especie.Value))
            {
                return "Espécie do protocolo é inválida.";
            }

            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return "Nome do protocolo é obrigatório.";
            }

            return null;
        }

        public async Task<ProtocoloDto> CadastrarAsync(SalvarProtocoloRequest request, CancellationToken cancellationToken = default)
        {
            var protocolo = new ProtocoloEntity();
            protocolo.Aplicar(request);

            await _repositorio.AdicionarAsync(protocolo, cancellationToken);

            return protocolo.ToDto();
        }

        public async Task<ProtocoloDto?> AtualizarAsync(
            long protocoloId,
            SalvarProtocoloRequest request,
            CancellationToken cancellationToken = default)
        {
            var protocolo = await _repositorio.ObterParaAlterarAsync(protocoloId, cancellationToken);

            if (protocolo is null)
            {
                return null;
            }

            protocolo.Aplicar(request);
            await _repositorio.AtualizarAsync(protocolo, cancellationToken);

            return protocolo.ToDto();
        }

        public async Task<bool> RemoverAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            var protocolo = await _repositorio.ObterParaAlterarAsync(protocoloId, cancellationToken);

            if (protocolo is null)
            {
                return false;
            }

            await _repositorio.RemoverAsync(protocolo, cancellationToken);

            return true;
        }
    }
}
