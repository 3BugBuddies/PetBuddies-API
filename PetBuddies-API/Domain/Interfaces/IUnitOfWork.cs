namespace PetBuddies_API.Domain.Interfaces
{
    /// <summary>
    /// Confirma no banco tudo que foi alterado pelos repositórios desde a última chamada.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SalvarAsync(CancellationToken cancellationToken = default);
    }
}
