using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface ILancamentoPontosRepository
    {
        Task<LancamentoPontosEntity?> ObterPorConsultaAsync(long consultaId, CancellationToken cancellationToken = default);

        Task<List<LancamentoPontosEntity>> ListarPorResponsavelAsync(long responsavelId, CancellationToken cancellationToken = default);

        Task<int> SomarPontosAsync(long responsavelId, CancellationToken cancellationToken = default);

        Task AdicionarAsync(LancamentoPontosEntity lancamento, CancellationToken cancellationToken = default);
    }
}
