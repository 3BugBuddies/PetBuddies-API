using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IRegraPontuacaoService
    {
        Task<List<RegraPontuacaoDto>> ListarAsync(
            int? clinicaId = null,
            TipoGestoEnum? gesto = null,
            CancellationToken cancellationToken = default);

        Task<RegraPontuacaoDto?> BuscarPorIdAsync(int regraId, CancellationToken cancellationToken = default);

        string? Validar(SalvarRegraPontuacaoRequest request);

        Task<bool> VigenciaExisteAsync(SalvarRegraPontuacaoRequest request, int? ignorarRegraId = null, CancellationToken cancellationToken = default);

        Task<RegraPontuacaoDto> CadastrarAsync(SalvarRegraPontuacaoRequest request, CancellationToken cancellationToken = default);
        Task<RegraPontuacaoDto?> AtualizarAsync(int regraId, SalvarRegraPontuacaoRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoverAsync(int regraId, CancellationToken cancellationToken = default);
    }
}
