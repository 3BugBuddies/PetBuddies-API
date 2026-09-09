using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class RegraPontuacaoRepository : IRegraPontuacaoRepository
    {
        private readonly ApplicationContext _context;

        public RegraPontuacaoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<RegraPontuacaoEntity>> ListarAsync(
            int? clinicaId = null,
            TipoGestoEnum? gesto = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.RegrasPontuacao.AsNoTracking();

            if (clinicaId.HasValue)
            {
                query = query.Where(regra => regra.ClinicaId == clinicaId.Value);
            }

            if (gesto.HasValue)
            {
                query = query.Where(regra => regra.Gesto == gesto.Value);
            }

            return query
                .OrderByDescending(regra => regra.InicioVigencia)
                .ThenBy(regra => regra.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<RegraPontuacaoEntity?> ObterPorIdAsync(int regraId, CancellationToken cancellationToken = default)
        {
            return _context.RegrasPontuacao
                .AsNoTracking()
                .SingleOrDefaultAsync(regra => regra.Id == regraId, cancellationToken);
        }

        public Task<RegraPontuacaoEntity?> ObterParaAlterarAsync(int regraId, CancellationToken cancellationToken = default)
        {
            return _context.RegrasPontuacao.SingleOrDefaultAsync(regra => regra.Id == regraId, cancellationToken);
        }

        public Task<bool> VigenciaExisteAsync(
            int clinicaId,
            TipoGestoEnum gesto,
            DateOnly inicioVigencia,
            int? ignorarRegraId = null,
            CancellationToken cancellationToken = default)
        {
            return _context.RegrasPontuacao
                .AsNoTracking()
                .AnyAsync(
                    regra => regra.ClinicaId == clinicaId
                        && regra.Gesto == gesto
                        && regra.InicioVigencia == inicioVigencia
                        && (ignorarRegraId == null || regra.Id != ignorarRegraId),
                    cancellationToken);
        }

        public async Task AdicionarAsync(RegraPontuacaoEntity regra, CancellationToken cancellationToken = default)
        {
            await _context.RegrasPontuacao.AddAsync(regra, cancellationToken);
        }

        public void Remover(RegraPontuacaoEntity regra)
        {
            _context.RegrasPontuacao.Remove(regra);
        }
    }
}
