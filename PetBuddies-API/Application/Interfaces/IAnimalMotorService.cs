using PetBuddies_API.Application.Dtos.Animal;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IAnimalMotorService
    {
        Task<AnimalMotorDto?> GetDadosMotorAsync(int animalId);
        Task<UltimaConsultaDto?> GetUltimaConsultaAsync(int animalId);
    }
}
