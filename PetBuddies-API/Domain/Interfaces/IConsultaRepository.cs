using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IConsultaRepository
    {
        /// <summary>Animal não é domínio deste repositório — checagem preservada aqui
        /// enquanto não existir um IAnimalRepository.</summary>
        Task<bool> AnimalExisteAsync(int animalId);

        /// <summary>Existe consulta ativa (não cancelada) para o veterinário no horário informado.
        /// Usado tanto pela checagem de ocupação de janela quanto pela checagem de vínculo
        /// de consulta em uma janela (JanelaAtendimento).</summary>
        Task<bool> ExisteAtivaAsync(int veterinarioId, DateTime dataHora, int? ignorarConsultaId = null);

        Task<bool> ExisteRealizadaAsync(int consultaId);

        /// <summary>A consulta de id informado pertence ao animal informado.</summary>
        Task<bool> PertenceAoAnimalAsync(int consultaId, int animalId);

        /// <summary>Pares (VeterinarioId, DataHora) de consultas não canceladas a partir de
        /// <paramref name="agora"/> — usado para excluir janelas já ocupadas da listagem de
        /// disponíveis (domínio JanelaAtendimento).</summary>
        Task<List<(int VeterinarioId, DateTime DataHora)>> ListarOcupacoesFuturasAsync(DateTime agora);

        Task<List<ConsultaEntity>> ListarAsync();
        Task<List<ConsultaEntity>> ListarPorAnimalAsync(int animalId);
        Task<ConsultaEntity?> ObterPorIdAsync(int consultaId);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<ConsultaEntity?> ObterParaAlterarAsync(int consultaId);

        Task AdicionarAsync(ConsultaEntity consulta);
        void Remover(ConsultaEntity consulta);
    }
}
