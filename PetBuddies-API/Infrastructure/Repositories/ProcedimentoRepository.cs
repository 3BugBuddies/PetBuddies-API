using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class ProcedimentoRepository : IProcedimentoRepository
    {
        private readonly ApplicationContext _context;

        public ProcedimentoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<ProcedimentoEntity>> ListarAsync(int? animalId = null)
        {
            var query = _context.Procedimentos.AsNoTracking();

            if (animalId.HasValue)
            {
                query = query.Where(procedimento => procedimento.AnimalId == animalId.Value);
            }

            return query
                .OrderByDescending(procedimento => procedimento.DataPrevistaInicio)
                .ToListAsync();
        }

        public Task<ProcedimentoEntity?> ObterPorIdAsync(int procedimentoId)
        {
            return _context.Procedimentos
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == procedimentoId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<ProcedimentoEntity?> ObterParaAlterarAsync(int procedimentoId)
        {
            return _context.Procedimentos.SingleOrDefaultAsync(item => item.Id == procedimentoId);
        }

        public Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return _context.Veterinarios
                .AsNoTracking()
                .AnyAsync(item => item.Id == veterinarioId);
        }

        public async Task AdicionarAsync(ProcedimentoEntity procedimento)
        {
            await _context.Procedimentos.AddAsync(procedimento);
        }

        public void Remover(ProcedimentoEntity procedimento)
        {
            _context.Procedimentos.Remove(procedimento);
        }
    }
}
