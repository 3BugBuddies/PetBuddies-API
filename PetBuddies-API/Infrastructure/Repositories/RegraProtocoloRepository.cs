using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class RegraProtocoloRepository : IRegraProtocoloRepository
    {
        private readonly ApplicationContext _context;

        public RegraProtocoloRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<RegraProtocoloEntity>> ListarPorProtocoloAsync(
            long protocoloId,
            CancellationToken cancellationToken = default)
        {
            return _context.RegrasProtocolo
                .AsNoTracking()
                .Where(regra => regra.ProtocoloId == protocoloId)
                .OrderBy(regra => regra.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<RegraProtocoloEntity?> ObterPorIdAsync(long regraId, CancellationToken cancellationToken = default)
        {
            return _context.RegrasProtocolo
                .AsNoTracking()
                .SingleOrDefaultAsync(regra => regra.Id == regraId, cancellationToken);
        }

        public Task<RegraProtocoloEntity?> ObterParaAlterarAsync(long regraId, CancellationToken cancellationToken = default)
        {
            return _context.RegrasProtocolo
                .SingleOrDefaultAsync(regra => regra.Id == regraId, cancellationToken);
        }

        public async Task AdicionarAsync(RegraProtocoloEntity regra, CancellationToken cancellationToken = default)
        {
            await _context.RegrasProtocolo.AddAsync(regra, cancellationToken);
        }

        public void Remover(RegraProtocoloEntity regra)
        {
            _context.RegrasProtocolo.Remove(regra);
        }
    }
}
