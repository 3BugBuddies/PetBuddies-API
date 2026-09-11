using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class OfertaRepository : IOfertaRepository
    {
        private readonly ApplicationContext _context;

        public OfertaRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<OfertaEntity>> ListarAsync(
            int? clinicaId = null,
            TipoAtoOfertaEnum? ato = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Ofertas.AsNoTracking();

            if (clinicaId.HasValue)
            {
                query = query.Where(oferta => oferta.ClinicaId == clinicaId.Value);
            }

            if (ato.HasValue)
            {
                query = query.Where(oferta => oferta.Ato == ato.Value);
            }

            return query
                .OrderByDescending(oferta => oferta.InicioVigencia)
                .ThenBy(oferta => oferta.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<OfertaEntity?> ObterPorIdAsync(int ofertaId, CancellationToken cancellationToken = default)
        {
            return _context.Ofertas
                .AsNoTracking()
                .SingleOrDefaultAsync(oferta => oferta.Id == ofertaId, cancellationToken);
        }

        public Task<OfertaEntity?> ObterParaAlterarAsync(int ofertaId, CancellationToken cancellationToken = default)
        {
            return _context.Ofertas.SingleOrDefaultAsync(oferta => oferta.Id == ofertaId, cancellationToken);
        }

        public Task<bool> VigenciaExisteAsync(
            int clinicaId,
            TipoAtoOfertaEnum ato,
            string? subtipo,
            long? protocoloId,
            DateOnly inicioVigencia,
            int? ignorarOfertaId = null,
            CancellationToken cancellationToken = default)
        {
            return _context.Ofertas
                .AsNoTracking()
                .AnyAsync(
                    oferta => oferta.ClinicaId == clinicaId
                        && oferta.Ato == ato
                        && oferta.Subtipo == subtipo
                        && oferta.ProtocoloId == protocoloId
                        && oferta.InicioVigencia == inicioVigencia
                        && (ignorarOfertaId == null || oferta.Id != ignorarOfertaId),
                    cancellationToken);
        }

        public async Task AdicionarAsync(OfertaEntity oferta, CancellationToken cancellationToken = default)
        {
            await _context.Ofertas.AddAsync(oferta, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoverAsync(OfertaEntity oferta, CancellationToken cancellationToken = default)
        {
            _context.Ofertas.Remove(oferta);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Entidade vem rastreada de ObterParaAlterarAsync (sem AsNoTracking): o EF já detecta
        // as mudanças, e chamar Update() marcaria todas as colunas como modificadas.
        public async Task AtualizarAsync(OfertaEntity oferta, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
