using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class OfertaService : IOfertaService
    {
        private readonly IOfertaRepository _repositorio;
        private readonly IProtocoloRepository _protocoloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public OfertaService(
            IOfertaRepository repositorio,
            IProtocoloRepository protocoloRepositorio,
            IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _protocoloRepositorio = protocoloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OfertaDto>> ListarAsync(
            int? clinicaId = null,
            TipoAtoOfertaEnum? ato = null,
            CancellationToken cancellationToken = default)
        {
            var ofertas = await _repositorio.ListarAsync(clinicaId, ato, cancellationToken);
            return ofertas.Select(oferta => oferta.ToDto()).ToList();
        }

        public async Task<OfertaDto?> BuscarPorIdAsync(int ofertaId, CancellationToken cancellationToken = default)
        {
            var oferta = await _repositorio.ObterPorIdAsync(ofertaId, cancellationToken);
            return oferta?.ToDto();
        }

        /// <summary>
        /// <para><b>O alvo tem de casar com o ato</b> (<c>CK_OFERTA_ALVO</c>): oferta de
        /// <c>PROTOCOLO</c> exige <c>ProtocoloId</c> e proíbe <c>Subtipo</c>; oferta de
        /// <c>CONSULTA</c> ou <c>PROCEDIMENTO</c> exige <c>Subtipo</c> e proíbe
        /// <c>ProtocoloId</c>. São duas colunas porque tipo de consulta e tipo de procedimento
        /// são enums com <c>CHECK</c>, não tabelas — não há catálogo para apontar.</para>
        ///
        /// <para><b>Valor não pode ser negativo</b> (<c>CK_OFERTA_VALOR</c>): preço negativo não
        /// é desconto, é erro de digitação.</para>
        ///
        /// <para>Não há validação de sobreposição de vigência: a vigência é por sucessão, e
        /// sem intervalo não existe o que sobrepor.</para>
        /// </summary>
        public string? Validar(SalvarOfertaRequest request)
        {
            if (request.Ato is null || !Enum.IsDefined(request.Ato.Value))
            {
                return "Tipo de ato da oferta é inválido.";
            }

            var temSubtipo = !string.IsNullOrWhiteSpace(request.Subtipo);
            var temProtocolo = request.ProtocoloId.HasValue;

            if (request.Ato.Value == TipoAtoOfertaEnum.PROTOCOLO)
            {
                if (!temProtocolo)
                {
                    return "Oferta de PROTOCOLO exige ProtocoloId.";
                }

                if (temSubtipo)
                {
                    return "Oferta de PROTOCOLO não aceita Subtipo: o alvo é a linha do catálogo.";
                }
            }
            else
            {
                if (!temSubtipo)
                {
                    return $"Oferta de {request.Ato.Value} exige Subtipo com o tipo do ato ofertado.";
                }

                if (temProtocolo)
                {
                    return $"Oferta de {request.Ato.Value} não aceita ProtocoloId.";
                }
            }

            if (request.Valor is null)
            {
                return "Valor da oferta é obrigatório.";
            }

            if (request.Valor.Value < 0)
            {
                return "Valor da oferta não pode ser negativo.";
            }

            if (request.InicioVigencia is null)
            {
                return "Início da vigência é obrigatório.";
            }

            if (string.IsNullOrWhiteSpace(request.Descricao))
            {
                return "Descrição da oferta é obrigatória.";
            }

            return null;
        }

        public Task<bool> ProtocoloExisteAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            return _protocoloRepositorio.ExisteAsync(protocoloId, cancellationToken);
        }

        public Task<bool> VigenciaExisteAsync(
            SalvarOfertaRequest request,
            int? ignorarOfertaId = null,
            CancellationToken cancellationToken = default)
        {
            var ato = request.Ato!.Value;
            var subtipo = ato == TipoAtoOfertaEnum.PROTOCOLO ? null : request.Subtipo!.Trim();
            var protocoloId = ato == TipoAtoOfertaEnum.PROTOCOLO ? request.ProtocoloId : null;

            return _repositorio.VigenciaExisteAsync(
                request.ClinicaId,
                ato,
                subtipo,
                protocoloId,
                request.InicioVigencia!.Value,
                ignorarOfertaId,
                cancellationToken);
        }

        public async Task<OfertaDto> CadastrarAsync(SalvarOfertaRequest request, CancellationToken cancellationToken = default)
        {
            var oferta = new OfertaEntity();
            oferta.Aplicar(request);

            await _repositorio.AdicionarAsync(oferta, cancellationToken);
            await _unitOfWork.SalvarAsync(cancellationToken);

            return oferta.ToDto();
        }

        public async Task<OfertaDto?> AtualizarAsync(
            int ofertaId,
            SalvarOfertaRequest request,
            CancellationToken cancellationToken = default)
        {
            var oferta = await _repositorio.ObterParaAlterarAsync(ofertaId, cancellationToken);

            if (oferta is null)
            {
                return null;
            }

            oferta.Aplicar(request);
            await _unitOfWork.SalvarAsync(cancellationToken);

            return oferta.ToDto();
        }

        public async Task<bool> RemoverAsync(int ofertaId, CancellationToken cancellationToken = default)
        {
            var oferta = await _repositorio.ObterParaAlterarAsync(ofertaId, cancellationToken);

            if (oferta is null)
            {
                return false;
            }

            _repositorio.Remover(oferta);
            await _unitOfWork.SalvarAsync(cancellationToken);

            return true;
        }
    }
}
