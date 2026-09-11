using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class RegraPontuacaoService : IRegraPontuacaoService
    {
        private readonly IRegraPontuacaoRepository _repositorio;

        public RegraPontuacaoService(IRegraPontuacaoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<RegraPontuacaoDto>> ListarAsync(
            int? clinicaId = null,
            TipoGestoEnum? gesto = null,
            CancellationToken cancellationToken = default)
        {
            var regras = await _repositorio.ListarAsync(clinicaId, gesto, cancellationToken);
            return regras.Select(regra => regra.ToDto()).ToList();
        }

        public async Task<RegraPontuacaoDto?> BuscarPorIdAsync(int regraId, CancellationToken cancellationToken = default)
        {
            var regra = await _repositorio.ObterPorIdAsync(regraId, cancellationToken);
            return regra?.ToDto();
        }

        // Espelha CK_PONTUACAO_PONTOS e CK_PONTUACAO_GESTO.
        public string? Validar(SalvarRegraPontuacaoRequest request)
        {
            if (request.Gesto is null || !Enum.IsDefined(request.Gesto.Value))
            {
                return "Gesto da regra de pontuação é inválido.";
            }

            if (request.Pontos is null)
            {
                return "Pontos são obrigatórios.";
            }

            if (request.Pontos.Value <= 0)
            {
                return "Pontos têm de ser positivos: gesto que tira ponto não existe no programa.";
            }

            if (request.InicioVigencia is null)
            {
                return "Início da vigência é obrigatório.";
            }

            return null;
        }

        // Espelha UK_PONTUACAO_VIGENCIA.
        public Task<bool> VigenciaExisteAsync(
            SalvarRegraPontuacaoRequest request,
            int? ignorarRegraId = null,
            CancellationToken cancellationToken = default)
        {
            return _repositorio.VigenciaExisteAsync(
                request.ClinicaId,
                request.Gesto!.Value,
                request.InicioVigencia!.Value,
                ignorarRegraId,
                cancellationToken);
        }

        public async Task<RegraPontuacaoDto> CadastrarAsync(
            SalvarRegraPontuacaoRequest request,
            CancellationToken cancellationToken = default)
        {
            var regra = new RegraPontuacaoEntity();
            regra.Aplicar(request);

            await _repositorio.AdicionarAsync(regra, cancellationToken);

            return regra.ToDto();
        }

        public async Task<RegraPontuacaoDto?> AtualizarAsync(
            int regraId,
            SalvarRegraPontuacaoRequest request,
            CancellationToken cancellationToken = default)
        {
            var regra = await _repositorio.ObterParaAlterarAsync(regraId, cancellationToken);

            if (regra is null)
            {
                return null;
            }

            regra.Aplicar(request);
            await _repositorio.AtualizarAsync(regra, cancellationToken);

            return regra.ToDto();
        }

        public async Task<bool> RemoverAsync(int regraId, CancellationToken cancellationToken = default)
        {
            var regra = await _repositorio.ObterParaAlterarAsync(regraId, cancellationToken);

            if (regra is null)
            {
                return false;
            }

            await _repositorio.RemoverAsync(regra, cancellationToken);

            return true;
        }
    }
}
