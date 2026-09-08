using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly ApplicationContext _context;

        public AnimalRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<AnimalEntity>> ListarAsync()
        {
            return _context.Animais
                .AsNoTracking()
                .OrderBy(animal => animal.Id)
                .ToListAsync();
        }

        public Task<AnimalEntity?> ObterPorIdAsync(int animalId)
        {
            return _context.Animais
                .AsNoTracking()
                .SingleOrDefaultAsync(animal => animal.Id == animalId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<AnimalEntity?> ObterParaAlterarAsync(int animalId)
        {
            return _context.Animais.SingleOrDefaultAsync(item => item.Id == animalId);
        }

        public Task<bool> ExisteAsync(int animalId)
        {
            return _context.Animais
                .AsNoTracking()
                .AnyAsync(animal => animal.Id == animalId);
        }

        public Task<bool> ResponsavelExisteAsync(int responsavelId)
        {
            return _context.Responsaveis
                .AsNoTracking()
                .AnyAsync(responsavel => responsavel.Id == responsavelId);
        }

        public Task<bool> PossuiConsultasAsync(int animalId)
        {
            return _context.Consultas
                .AsNoTracking()
                .AnyAsync(consulta => consulta.AnimalId == animalId);
        }

        public async Task AdicionarAsync(AnimalEntity animal)
        {
            await _context.Animais.AddAsync(animal);
        }

        public void Remover(AnimalEntity animal)
        {
            _context.Animais.Remove(animal);
        }

        /// <summary>
        /// A projecao para DTO foi movida para o caso de uso: o repositorio vive em
        /// Domain, e Domain nao pode conhecer os DTOs de Application — a dependencia
        /// da Clean Architecture aponta para dentro, nunca para fora.
        /// </summary>
        public Task<AnimalEntity?> ObterDadosMotorAsync(int animalId)
        {
            return _context.Animais
                .AsNoTracking()
                .SingleOrDefaultAsync(animal => animal.Id == animalId);
        }

        public Task<ConsultaEntity?> ObterUltimaConsultaRealizadaAsync(int animalId)
        {
            return _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.AnimalId == animalId
                                && consulta.Status == StatusConsultaEnum.REALIZADA)
                .OrderByDescending(consulta => consulta.DataHora)
                .FirstOrDefaultAsync();
        }
    }
}
