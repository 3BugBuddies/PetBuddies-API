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

        /// <summary>Pontos positivos e gesto conhecido. Devolve <c>null</c> quando é válido.</summary>
        string? Validar(SalvarRegraPontuacaoRequest request);

        /// <summary>Um valor por clínica, gesto e vigência.</summary>
        Task<bool> VigenciaExisteAsync(SalvarRegraPontuacaoRequest request, int? ignorarRegraId = null, CancellationToken cancellationToken = default);

        Task<RegraPontuacaoDto> CadastrarAsync(SalvarRegraPontuacaoRequest request, CancellationToken cancellationToken = default);
        Task<RegraPontuacaoDto?> AtualizarAsync(int regraId, SalvarRegraPontuacaoRequest request, CancellationToken cancellationToken = default);
        Task<bool> RemoverAsync(int regraId, CancellationToken cancellationToken = default);
    }
}
