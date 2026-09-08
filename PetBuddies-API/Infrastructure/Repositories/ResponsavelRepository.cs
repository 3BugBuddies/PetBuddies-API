using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Data;

namespace PetBuddies_API.Infrastructure.Repositories
{
    public class ResponsavelRepository : IResponsavelRepository
    {
        private readonly ApplicationContext _context;

        public ResponsavelRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<ResponsavelEntity?> ObterPorTelefoneAsync(string telefoneNormalizado)
        {
            return _context.Responsaveis
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Telefone == telefoneNormalizado);
        }

        public Task<List<ResponsavelEntity>> ListarAsync()
        {
            return _context.Responsaveis
                .AsNoTracking()
                .OrderBy(item => item.Id)
                .ToListAsync();
        }

        public Task<ResponsavelEntity?> ObterPorIdAsync(int responsavelId)
        {
            return _context.Responsaveis
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == responsavelId);
        }

        /// <summary>Sem <c>AsNoTracking</c>: o caso de uso vai alterar ou remover a entidade.</summary>
        public Task<ResponsavelEntity?> ObterParaAlterarAsync(int responsavelId)
        {
            return _context.Responsaveis.SingleOrDefaultAsync(item => item.Id == responsavelId);
        }

        public Task<bool> TelefoneExisteAsync(string telefoneNormalizado, int? ignorarResponsavelId = null)
        {
            var query = _context.Responsaveis
                .AsNoTracking()
                .Where(item => item.Telefone == telefoneNormalizado);

            if (ignorarResponsavelId.HasValue)
                query = query.Where(item => item.Id != ignorarResponsavelId.Value);

            return query.AnyAsync();
        }

        public Task<bool> PossuiAnimaisAsync(int responsavelId)
        {
            return _context.Animais
                .AsNoTracking()
                .AnyAsync(animal => animal.ResponsavelId == responsavelId);
        }

        public Task<bool> ExisteAsync(int responsavelId)
        {
            return _context.Responsaveis
                .AsNoTracking()
                .AnyAsync(item => item.Id == responsavelId);
        }

        /// <summary>A projecao para DTO e do caso de uso; Domain nao conhece Application.</summary>
        public Task<List<AnimalEntity>> ListarAnimaisAsync(int responsavelId)
        {
            return _context.Animais
                .AsNoTracking()
                .Where(animal => animal.ResponsavelId == responsavelId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(ResponsavelEntity responsavel)
        {
            await _context.Responsaveis.AddAsync(responsavel);
        }

        public void Remover(ResponsavelEntity responsavel)
        {
            _context.Responsaveis.Remove(responsavel);
        }
    }
}
