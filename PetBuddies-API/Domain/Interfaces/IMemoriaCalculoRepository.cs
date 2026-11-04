using PetBuddies_API.Domain.Documents;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IMemoriaCalculoRepository
    {
        Task SalvarAsync(MemoriaCalculoDocument memoria, CancellationToken cancellationToken = default);

        Task<MemoriaCalculoDocument?> ObterPorConsultaAsync(long consultaId, CancellationToken cancellationToken = default);
    }
}
