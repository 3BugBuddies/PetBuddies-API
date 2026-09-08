using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class VeterinarioRepository : IVeterinarioRepository
    {
        private readonly ApplicationContext _context;

        public VeterinarioRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<List<VeterinarioEntity>> ListarAsync()
        {
            return _context.Veterinarios
                .AsNoTracking()
                .OrderBy(veterinario => veterinario.Id)
                .ToListAsync();
        }

        public Task<VeterinarioEntity?> ObterPorIdAsync(int veterinarioId)
        {
            return _context.Veterinarios
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == veterinarioId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<VeterinarioEntity?> ObterParaAlterarAsync(int veterinarioId)
        {
            return _context.Veterinarios.SingleOrDefaultAsync(item => item.Id == veterinarioId);
        }

        public Task<List<VeterinarioEntity>> ListarPorClinicaAsync(int clinicaId)
        {
            return _context.Veterinarios
                .AsNoTracking()
                .Where(item => item.ClinicaId == clinicaId)
                .OrderBy(item => item.Nome)
                .ToListAsync();
        }

        public Task<bool> ClinicaExisteAsync(int clinicaId)
        {
            return _context.Clinicas
                .AsNoTracking()
                .AnyAsync(item => item.Id == clinicaId);
        }

        /// <summary>
        /// O filtro por clínica foi removido num PR anterior — mantido como está,
        /// o CRMV é validado apenas globalmente.
        /// </summary>
        public Task<bool> CrmvExisteAsync(string crmv, int? ignorarVeterinarioId = null)
        {
            var crmvNormalizado = crmv.Trim();

            var query = _context.Veterinarios
                .AsNoTracking()
                .Where(item => item.Crmv == crmvNormalizado);

            if (ignorarVeterinarioId.HasValue)
                query = query.Where(item => item.Id != ignorarVeterinarioId.Value);

            return query.AnyAsync();
        }

        public async Task AdicionarAsync(VeterinarioEntity veterinario)
        {
            await _context.Veterinarios.AddAsync(veterinario);
        }

        public void Remover(VeterinarioEntity veterinario)
        {
            _context.Veterinarios.Remove(veterinario);
        }
    }
}
