using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IRegistroAtendimentoRepository
    {
        Task<List<RegistroAtendimentoEntity>> ListarAsync(int? animalId = null);
        Task<RegistroAtendimentoEntity?> ObterPorIdAsync(int registroAtendimentoId);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<RegistroAtendimentoEntity?> ObterParaAlterarAsync(int registroAtendimentoId);

        /// <summary>Animal não é domínio deste repositório — checagem preservada aqui
        /// enquanto não existir um IAnimalRepository.</summary>
        Task<bool> AnimalExisteAsync(int animalId);

        /// <summary>ConsultaId do registro de atendimento — usado pelo domínio Procedimento
        /// para disparar o plano pós-cirúrgico no Java.</summary>
        Task<int> ObterConsultaIdAsync(int registroAtendimentoId);

        /// <summary>O registro de atendimento de id informado pertence ao animal informado.</summary>
        Task<bool> PertenceAoAnimalAsync(int registroAtendimentoId, int animalId);

        Task AdicionarAsync(RegistroAtendimentoEntity registro);
        void Remover(RegistroAtendimentoEntity registro);
    }
}
