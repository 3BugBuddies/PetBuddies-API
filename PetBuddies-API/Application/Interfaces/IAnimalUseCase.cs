using PetBuddies_API.Application.Dtos.Animal;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IAnimalUseCase
    {
        Task<List<AnimalDto>> ListarAsync();
        Task<AnimalDto?> BuscarPorIdAsync(int animalId);
        Task<bool> ExisteAsync(int animalId);
        Task<bool> ResponsavelExisteAsync(int responsavelId);
        Task<bool> PossuiConsultasAsync(int animalId);
        Task<AnimalDto> CadastrarAsync(CadastrarAnimalRequest request);
        Task<AnimalDto?> AtualizarAsync(int animalId, AtualizarAnimalRequest request);
        Task<bool> RemoverAsync(int animalId);
    }
}
