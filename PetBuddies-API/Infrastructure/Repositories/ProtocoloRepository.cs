using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class ProtocoloRepository : IProtocoloRepository
    {
        private readonly ApplicationContext _context;

        public ProtocoloRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<ProtocoloEntity>> ListarAsync(
            EspecieEnum? especie = null,
            CategoriaProtocoloEnum? categoria = null,
            bool? ativo = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Protocolos
                .AsNoTracking()
                .Include(protocolo => protocolo.Regras)
                .AsQueryable();

            if (especie.HasValue)
            {
                query = query.Where(protocolo => protocolo.Especie == especie.Value);
            }

            if (categoria.HasValue)
            {
                query = query.Where(protocolo => protocolo.Categoria == categoria.Value);
            }

            if (ativo.HasValue)
            {
                query = query.Where(protocolo => protocolo.Ativo == ativo.Value);
            }

            return query
                .OrderBy(protocolo => protocolo.Nome)
                .ToListAsync(cancellationToken);
        }

        public Task<ProtocoloEntity?> ObterPorIdAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            return _context.Protocolos
                .AsNoTracking()
                .Include(protocolo => protocolo.Regras)
                .SingleOrDefaultAsync(protocolo => protocolo.Id == protocoloId, cancellationToken);
        }

        public Task<ProtocoloEntity?> ObterParaAlterarAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            return _context.Protocolos
                .SingleOrDefaultAsync(protocolo => protocolo.Id == protocoloId, cancellationToken);
        }

        public Task<bool> ExisteAsync(long protocoloId, CancellationToken cancellationToken = default)
        {
            return _context.Protocolos
                .AsNoTracking()
                .AnyAsync(protocolo => protocolo.Id == protocoloId, cancellationToken);
        }

        public async Task AdicionarAsync(ProtocoloEntity protocolo, CancellationToken cancellationToken = default)
        {
            await _context.Protocolos.AddAsync(protocolo, cancellationToken);
        }

        public void Remover(ProtocoloEntity protocolo)
        {
            _context.Protocolos.Remove(protocolo);
        }
    }
}
