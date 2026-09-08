using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Animal;
using PetBuddies_API.Enums;

namespace PetBuddies_API.Services
{
    public class AnimalMotorService
    {
        private readonly ApplicationContext _context;

        public AnimalMotorService(ApplicationContext context)
        {
            _context = context;
        }

        public Task<AnimalMotorDto?> GetDadosMotorAsync(int animalId)
        {
            return _context.Animais
                .AsNoTracking()
                .Where(animal => animal.Id == animalId)
                .Select(animal => new AnimalMotorDto
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
                })
                .SingleOrDefaultAsync();
        }

        public Task<UltimaConsultaDto?> GetUltimaConsultaAsync(int animalId)
        {
            return _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.AnimalId == animalId && consulta.Status == StatusConsultaEnum.REALIZADA)
                .OrderByDescending(consulta => consulta.DataHora)
                .Select(consulta => new UltimaConsultaDto
                {
                    DataHora = consulta.DataHora,
                    Tipo = consulta.TipoConsulta.ToString()
                })
                .FirstOrDefaultAsync();
        }
    }
}
