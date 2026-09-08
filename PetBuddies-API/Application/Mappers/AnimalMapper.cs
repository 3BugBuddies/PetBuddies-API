using PetBuddies_API.Application.Dtos.Animal;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class AnimalMapper
    {
        public static AnimalDto ToDto(this AnimalEntity animal)
        {
            return new AnimalDto
            {
                Id = animal.Id,
                Nome = animal.Nome,
                Especie = animal.Especie.ToString(),
                Raca = animal.Raca,
                Porte = animal.Porte.ToString(),
                Sexo = animal.Sexo.ToString(),
                Castrado = animal.Castrado,
                CondicaoCronica = animal.CondicaoCronica,
                DataNascimento = animal.DataNascimento,
                Peso = animal.Peso,
                Alergias = animal.Alergias,
                Observacoes = animal.Observacoes
            };
        }
    
        /// <summary>Projeção enxuta que o motor de cuidado do serviço Java consome.</summary>
        public static AnimalMotorDto ToMotorDto(this AnimalEntity animal)
        {
            return new AnimalMotorDto
            {
                Id = animal.Id,
                Nome = animal.Nome,
                DataNascimento = animal.DataNascimento,
                CondicaoCronica = animal.CondicaoCronica,
                Castrado = animal.Castrado,
                Sexo = animal.Sexo.ToString(),
                Especie = animal.Especie.ToString(),
                Raca = animal.Raca,
                Porte = animal.Porte.ToString()
            };
        }
    }
}
