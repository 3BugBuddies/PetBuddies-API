using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class LancamentoPontosRepository : ILancamentoPontosRepository
    {
        private readonly ApplicationContext _context;

        public LancamentoPontosRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<LancamentoPontosEntity?> ObterPorConsultaAsync(long consultaId, CancellationToken cancellationToken = default)
        {
            return _context.LancamentosPontos
                .AsNoTracking()
                .SingleOrDefaultAsync(lancamento => lancamento.ConsultaId == consultaId, cancellationToken);
        }

        public Task<List<LancamentoPontosEntity>> ListarPorResponsavelAsync(long responsavelId, CancellationToken cancellationToken = default)
        {
            return _context.LancamentosPontos
                .AsNoTracking()
                .Where(lancamento => lancamento.ResponsavelId == responsavelId)
                .OrderByDescending(lancamento => lancamento.DataConsulta)
                .ThenByDescending(lancamento => lancamento.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<int> SomarPontosAsync(long responsavelId, CancellationToken cancellationToken = default)
        {
            return _context.LancamentosPontos
                .Where(lancamento => lancamento.ResponsavelId == responsavelId)
                .SumAsync(lancamento => lancamento.Pontos, cancellationToken);
        }

        public async Task AdicionarAsync(LancamentoPontosEntity lancamento, CancellationToken cancellationToken = default)
        {
            await _context.LancamentosPontos.AddAsync(lancamento, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
