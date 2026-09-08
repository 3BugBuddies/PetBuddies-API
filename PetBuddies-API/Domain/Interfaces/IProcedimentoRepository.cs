using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IProcedimentoRepository
    {
        Task<List<ProcedimentoEntity>> ListarAsync(int? animalId = null);
        Task<ProcedimentoEntity?> ObterPorIdAsync(int procedimentoId);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<ProcedimentoEntity?> ObterParaAlterarAsync(int procedimentoId);

        /// <summary>Animal não é domínio deste repositório — checagem preservada aqui
        /// enquanto não existir um IAnimalRepository.</summary>
        Task<bool> AnimalExisteAsync(int animalId);

        /// <summary>Veterinário não é domínio deste repositório — checagem preservada aqui
        /// enquanto não existir um IVeterinarioRepository.</summary>
        Task<bool> VeterinarioExisteAsync(int veterinarioId);

        Task AdicionarAsync(ProcedimentoEntity procedimento);
        void Remover(ProcedimentoEntity procedimento);
    }
}
