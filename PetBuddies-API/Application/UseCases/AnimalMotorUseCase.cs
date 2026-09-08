using PetBuddies_API.Application.Dtos.Animal;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    /// <summary>
    /// Projecao que o motor de cuidado do servico Java consome. So leitura — nao
    /// depende de IUnitOfWork.
    /// </summary>
    public class AnimalMotorUseCase : IAnimalMotorUseCase
    {
        private readonly IAnimalRepository _repositorio;

        public AnimalMotorUseCase(IAnimalRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<AnimalMotorDto?> GetDadosMotorAsync(int animalId)
        {
            var animal = await _repositorio.ObterDadosMotorAsync(animalId);
            return animal?.ToMotorDto();
        }

        public async Task<UltimaConsultaDto?> GetUltimaConsultaAsync(int animalId)
        {
            var consulta = await _repositorio.ObterUltimaConsultaRealizadaAsync(animalId);

            return consulta is null ? null : new UltimaConsultaDto
            {
                DataHora = consulta.DataHora,
                Tipo = consulta.TipoConsulta.ToString()
            };
        }
    }
}
