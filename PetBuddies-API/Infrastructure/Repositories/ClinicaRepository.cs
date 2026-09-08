using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class ClinicaRepository : IClinicaRepository
    {
        private readonly ApplicationContext _context;

        public ClinicaRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<ClinicaEntity?> ObterPorIdAsync(int clinicaId)
        {
            return _context.Clinicas
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == clinicaId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<ClinicaEntity?> ObterParaAlterarAsync(int clinicaId)
        {
            return _context.Clinicas.SingleOrDefaultAsync(item => item.Id == clinicaId);
        }

        public Task<List<ClinicaEntity>> ListarAsync()
        {
            return _context.Clinicas
                .AsNoTracking()
                .OrderBy(clinica => clinica.Id)
                .ToListAsync();
        }

        public Task<List<ClinicaEntity>> BuscarPorNomeAsync(string nome)
        {
            return _context.Clinicas
                .AsNoTracking()
                .Where(item => item.Nome.ToLower().Contains(nome.ToLower()))
                .OrderBy(item => item.Nome)
                .ToListAsync();
        }

        public Task<bool> CnpjExisteAsync(string cnpj, int? ignorarClinicaId = null)
        {
            var cnpjNormalizado = cnpj.Trim();

            var query = _context.Clinicas
                .AsNoTracking()
                .Where(item => item.Cnpj == cnpjNormalizado);

            if (ignorarClinicaId.HasValue)
                query = query.Where(item => item.Id != ignorarClinicaId.Value);

            return query.AnyAsync();
        }

        public async Task AdicionarAsync(ClinicaEntity clinica)
        {
            await _context.Clinicas.AddAsync(clinica);
        }

        public void Remover(ClinicaEntity clinica)
        {
            _context.Clinicas.Remove(clinica);
        }
    }
}
