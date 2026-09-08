using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IAnimalRepository
    {
        Task<List<AnimalEntity>> ListarAsync();
        Task<AnimalEntity?> ObterPorIdAsync(int animalId);
        Task<AnimalEntity?> ObterParaAlterarAsync(int animalId);
        Task<bool> ExisteAsync(int animalId);
        Task<bool> ResponsavelExisteAsync(int responsavelId);
        Task<bool> PossuiConsultasAsync(int animalId);
        Task AdicionarAsync(AnimalEntity animal);
        void Remover(AnimalEntity animal);

        /// <summary>Projeção consumida pelo motor de cuidado (Java).</summary>
        Task<AnimalEntity?> ObterDadosMotorAsync(int animalId);

        /// <summary>
        /// Consulta a tabela de Consulta (fora do domínio Animal), mas o método fica aqui
        /// por instrução explícita da tarefa — não criar um IConsultaRepository só para isto.
        /// </summary>
        Task<ConsultaEntity?> ObterUltimaConsultaRealizadaAsync(int animalId);
    }
}
