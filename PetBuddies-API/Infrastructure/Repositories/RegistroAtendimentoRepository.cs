using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class RegistroAtendimentoRepository : IRegistroAtendimentoRepository
    {
        private readonly ApplicationContext _context;

        public RegistroAtendimentoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<RegistroAtendimentoEntity>> ListarAsync(int? animalId = null)
        {
            var query = _context.RegistrosAtendimento.AsNoTracking();

            if (animalId.HasValue)
            {
                query = query.Where(registro => registro.AnimalId == animalId.Value);
            }

            return query
                .OrderByDescending(registro => registro.DataAtendimento)
                .ToListAsync();
        }

        public Task<RegistroAtendimentoEntity?> ObterPorIdAsync(int registroAtendimentoId)
        {
            return _context.RegistrosAtendimento
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == registroAtendimentoId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<RegistroAtendimentoEntity?> ObterParaAlterarAsync(int registroAtendimentoId)
        {
            return _context.RegistrosAtendimento.SingleOrDefaultAsync(item => item.Id == registroAtendimentoId);
        }

        public Task<bool> AnimalExisteAsync(int animalId)
        {
            return _context.Animais
                .AsNoTracking()
                .AnyAsync(item => item.Id == animalId);
        }

        public Task<int> ObterConsultaIdAsync(int registroAtendimentoId)
        {
            return _context.RegistrosAtendimento
                .AsNoTracking()
                .Where(registro => registro.Id == registroAtendimentoId)
                .Select(registro => registro.ConsultaId)
                .SingleAsync();
        }

        public Task<bool> PertenceAoAnimalAsync(int registroAtendimentoId, int animalId)
        {
            return _context.RegistrosAtendimento
                .AsNoTracking()
                .AnyAsync(item => item.Id == registroAtendimentoId && item.AnimalId == animalId);
        }

        public async Task AdicionarAsync(RegistroAtendimentoEntity registro)
        {
            await _context.RegistrosAtendimento.AddAsync(registro);
        }

        public void Remover(RegistroAtendimentoEntity registro)
        {
            _context.RegistrosAtendimento.Remove(registro);
        }
    }
}
