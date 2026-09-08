using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly ApplicationContext _context;

        public ConsultaRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<bool> ExisteAtivaAsync(int veterinarioId, DateTime dataHora, int? ignorarConsultaId = null)
        {
            var query = _context.Consultas
                .AsNoTracking()
                .Where(consulta =>
                    consulta.VeterinarioId == veterinarioId
                    && consulta.DataHora == dataHora
                    && consulta.Status != StatusConsultaEnum.CANCELADA);

            if (ignorarConsultaId.HasValue)
                query = query.Where(consulta => consulta.Id != ignorarConsultaId.Value);

            return query.AnyAsync();
        }

        public Task<bool> ExisteRealizadaAsync(int consultaId)
        {
            return _context.Consultas
                .AsNoTracking()
                .AnyAsync(consulta => consulta.Id == consultaId && consulta.Status == StatusConsultaEnum.REALIZADA);
        }

        public Task<bool> PertenceAoAnimalAsync(int consultaId, int animalId)
        {
            return _context.Consultas
                .AsNoTracking()
                .AnyAsync(item => item.Id == consultaId && item.AnimalId == animalId);
        }

        public async Task<List<(int VeterinarioId, DateTime DataHora)>> ListarOcupacoesFuturasAsync(DateTime agora)
        {
            var consultasOcupadas = await _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.Status != StatusConsultaEnum.CANCELADA && consulta.DataHora >= agora)
                .Select(consulta => new { consulta.VeterinarioId, consulta.DataHora })
                .ToListAsync();

            return consultasOcupadas
                .Select(consulta => (consulta.VeterinarioId, consulta.DataHora))
                .ToList();
        }

        public Task<List<ConsultaEntity>> ListarAsync()
        {
            return _context.Consultas
                .AsNoTracking()
                .OrderByDescending(consulta => consulta.DataHora)
                .ToListAsync();
        }

        public Task<List<ConsultaEntity>> ListarPorAnimalAsync(int animalId)
        {
            return _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.AnimalId == animalId)
                .OrderByDescending(consulta => consulta.DataHora)
                .ToListAsync();
        }

        public Task<ConsultaEntity?> ObterPorIdAsync(int consultaId)
        {
            return _context.Consultas
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == consultaId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<ConsultaEntity?> ObterParaAlterarAsync(int consultaId)
        {
            return _context.Consultas.SingleOrDefaultAsync(item => item.Id == consultaId);
        }

        public async Task AdicionarAsync(ConsultaEntity consulta)
        {
            await _context.Consultas.AddAsync(consulta);
        }

        public void Remover(ConsultaEntity consulta)
        {
            _context.Consultas.Remove(consulta);
        }
    }
}
