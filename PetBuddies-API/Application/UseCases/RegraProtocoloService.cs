using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class RegraProtocoloService : IRegraProtocoloService
    {
        private readonly IRegraProtocoloRepository _repositorio;
        private readonly IProtocoloRepository _protocoloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public RegraProtocoloService(
            IRegraProtocoloRepository repositorio,
            IProtocoloRepository protocoloRepositorio,
            IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _protocoloRepositorio = protocoloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RegraProtocoloDto>> ListarPorProtocoloAsync(
            long protocoloId,
            CancellationToken cancellationToken = default)
        {
            var regras = await _repositorio.ListarPorProtocoloAsync(protocoloId, cancellationToken);
            return regras.Select(regra => regra.ToDto()).ToList();
        }

        public async Task<RegraProtocoloDto?> BuscarPorIdAsync(long regraId, CancellationToken cancellationToken = default)
        {
            var regra = await _repositorio.ObterPorIdAsync(regraId, cancellationToken);
            return regra?.ToDto();
        }

        // Espelha CK_REGPROT_REPETICOES e CK_REGPROT_RECORRENCIA.
        public string? Validar(SalvarRegraProtocoloRequest request)
        {
            if (request.Tipo is null || !Enum.IsDefined(request.Tipo.Value))
            {
                return "Tipo de cuidado da regra é inválido.";
            }

            if (request.UnidadeOffset is null || !Enum.IsDefined(request.UnidadeOffset.Value))
            {
                return "Unidade do deslocamento é inválida.";
            }

            if (request.DataBase is null || !Enum.IsDefined(request.DataBase.Value))
            {
                return "Data-base da regra é inválida.";
            }

            if (request.Offset is null)
            {
                return "Deslocamento da regra é obrigatório.";
            }

            if (request.Repeticoes is null)
            {
                return "Número de repetições é obrigatório: regra sem repetições expandiria "
                    + "indefinidamente na criação do plano.";
            }

            if (request.Repeticoes.Value < 1)
            {
                return "Número de repetições deve ser pelo menos 1.";
            }

            var temIntervalo = request.Intervalo.HasValue;
            var temUnidadeIntervalo = request.UnidadeIntervalo.HasValue;

            if (temIntervalo != temUnidadeIntervalo)
            {
                return "Intervalo e unidade de intervalo têm de vir juntos: preencha os dois "
                    + "para regra recorrente, ou nenhum para ocorrência única.";
            }

            if (temUnidadeIntervalo && !Enum.IsDefined(request.UnidadeIntervalo!.Value))
            {
                return "Unidade do intervalo é inválida.";
            }

            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return "Nome da regra é obrigatório.";
            }

            return null;
        }

        public Task<bool> ProtocoloExisteAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            return _protocoloRepositorio.ExisteAsync(protocoloId, cancellationToken);
        }

        public async Task<RegraProtocoloDto> CadastrarAsync(
            SalvarRegraProtocoloRequest request,
            CancellationToken cancellationToken = default)
        {
            var regra = new RegraProtocoloEntity();
            regra.Aplicar(request);

            await _repositorio.AdicionarAsync(regra, cancellationToken);
            await _unitOfWork.SalvarAsync(cancellationToken);

            return regra.ToDto();
        }

        public async Task<RegraProtocoloDto?> AtualizarAsync(
            long regraId,
            SalvarRegraProtocoloRequest request,
            CancellationToken cancellationToken = default)
        {
            var regra = await _repositorio.ObterParaAlterarAsync(regraId, cancellationToken);

            if (regra is null)
            {
                return null;
            }

            regra.Aplicar(request);
            await _unitOfWork.SalvarAsync(cancellationToken);

            return regra.ToDto();
        }

        public async Task<bool> RemoverAsync(long regraId, CancellationToken cancellationToken = default)
        {
            var regra = await _repositorio.ObterParaAlterarAsync(regraId, cancellationToken);

            if (regra is null)
            {
                return false;
            }

            _repositorio.Remover(regra);
            await _unitOfWork.SalvarAsync(cancellationToken);

            return true;
        }
    }
}
