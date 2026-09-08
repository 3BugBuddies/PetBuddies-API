using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IProcedimentoRepository
    {
        Task<List<ProcedimentoEntity>> ListarAsync(int? animalId = null);
        Task<ProcedimentoEntity?> ObterPorIdAsync(int procedimentoId);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<ProcedimentoEntity?> ObterParaAlterarAsync(int procedimentoId);

        /// <summary>Veterinário não é domínio deste repositório. IVeterinarioRepository já existe,
        /// mas não expõe um ExisteAsync genérico por id — checagem preservada aqui até que exponha.</summary>
        Task<bool> VeterinarioExisteAsync(int veterinarioId);

        Task AdicionarAsync(ProcedimentoEntity procedimento);
        void Remover(ProcedimentoEntity procedimento);
    }
}
