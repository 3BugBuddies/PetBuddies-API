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
                .Include(animal => animal.TipoAnimal)
                .Where(animal => animal.Id == animalId)
                .Select(animal => new AnimalMotorDto
                {
                    Id = animal.Id,
                    Nome = animal.Nome,
                    DataNascimento = animal.DataNascimento,
                    CondicaoCronica = animal.CondicaoCronica,
                    Castrado = animal.Castrado,
                    PreCadastro = animal.PreCadastro,
                    Sexo = animal.Sexo.ToString(),
                    Especie = animal.TipoAnimal != null ? animal.TipoAnimal.Especie.ToString() : string.Empty,
                    Porte = animal.TipoAnimal != null ? animal.TipoAnimal.Porte.ToString() : string.Empty
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
