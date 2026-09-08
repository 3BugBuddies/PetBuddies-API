using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class JanelaAtendimentoRepository : IJanelaAtendimentoRepository
    {
        private readonly ApplicationContext _context;

        public JanelaAtendimentoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<JanelaAtendimentoEntity?> ObterPorIdAsync(int janelaId)
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .Include(item => item.Veterinario)
                .SingleOrDefaultAsync(item => item.Id == janelaId);
        }

        public Task<JanelaAtendimentoEntity?> ObterParaVerificacaoAsync(int janelaId)
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == janelaId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<JanelaAtendimentoEntity?> ObterParaAlterarAsync(int janelaId)
        {
            return _context.JanelasAtendimento.SingleOrDefaultAsync(item => item.Id == janelaId);
        }

        public Task<bool> ExisteAsync(int janelaId)
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .AnyAsync(janela => janela.Id == janelaId);
        }

        public Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return _context.Veterinarios
                .AsNoTracking()
                .AnyAsync(item => item.Id == veterinarioId);
        }

        public Task<bool> HorarioExisteAsync(int veterinarioId, DateTime dataHoraInicio, int? ignorarJanelaId = null)
        {
            var query = _context.JanelasAtendimento
                .AsNoTracking()
                .Where(item =>
                    item.VeterinarioId == veterinarioId
                    && item.DataHoraInicio == dataHoraInicio);

            if (ignorarJanelaId.HasValue)
                query = query.Where(item => item.Id != ignorarJanelaId.Value);

            return query.AnyAsync();
        }

        public Task<List<JanelaAtendimentoEntity>> ListarAsync()
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .Include(janela => janela.Veterinario)
                .OrderBy(janela => janela.DataHoraInicio)
                .ToListAsync();
        }

        public Task<List<JanelaAtendimentoEntity>> ListarFuturasAsync(DateTime agora)
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .Include(janela => janela.Veterinario)
                .Where(janela => janela.DataHoraInicio >= agora)
                .OrderBy(janela => janela.DataHoraInicio)
                .ToListAsync();
        }

        public async Task AdicionarAsync(JanelaAtendimentoEntity janela)
        {
            await _context.JanelasAtendimento.AddAsync(janela);
        }

        public void Remover(JanelaAtendimentoEntity janela)
        {
            _context.JanelasAtendimento.Remove(janela);
        }
    }
}
