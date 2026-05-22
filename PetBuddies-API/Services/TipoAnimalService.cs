using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.TipoAnimal;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class TipoAnimalService
    {
        private readonly ApplicationContext _context;

        public TipoAnimalService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<TipoAnimalDto>> ListarAsync()
        {
            var tipos = await _context.TiposAnimal
                .AsNoTracking()
                .OrderBy(tipo => tipo.Id)
                .ToListAsync();

            return tipos.Select(ToDto).ToList();
        }

        public async Task<TipoAnimalDto?> BuscarPorIdAsync(int tipoAnimalId)
        {
            var tipo = await _context.TiposAnimal
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == tipoAnimalId);

            return tipo is null ? null : ToDto(tipo);
        }

        public async Task<bool> DuplicadoAsync(SalvarTipoAnimalRequest request, int? ignorarTipoAnimalId = null)
        {
            var raca = request.Raca.Trim();

            if (string.IsNullOrWhiteSpace(raca))
            {
                return false;
            }

            var query = _context.TiposAnimal
                .AsNoTracking()
                .Where(item =>
                    item.Especie == request.Especie!.Value
                    && item.Porte == request.Porte!.Value
                    && item.Raca == raca);

            if (ignorarTipoAnimalId.HasValue)
                query = query.Where(item => item.Id != ignorarTipoAnimalId.Value);

            return await query.AnyAsync();
        }

        public async Task<TipoAnimalDto> CadastrarAsync(SalvarTipoAnimalRequest request)
        {
            var tipo = new TipoAnimalEntity();
            Aplicar(tipo, request);

            _context.TiposAnimal.Add(tipo);
            await _context.SaveChangesAsync();

            return ToDto(tipo);
        }

        public async Task<TipoAnimalDto?> AtualizarAsync(int tipoAnimalId, SalvarTipoAnimalRequest request)
        {
            var tipo = await _context.TiposAnimal.SingleOrDefaultAsync(item => item.Id == tipoAnimalId);

            if (tipo is null)
            {
                return null;
            }

            Aplicar(tipo, request);
            await _context.SaveChangesAsync();

            return ToDto(tipo);
        }

        public async Task<bool> RemoverAsync(int tipoAnimalId)
        {
            var tipo = await _context.TiposAnimal.SingleOrDefaultAsync(item => item.Id == tipoAnimalId);

            if (tipo is null)
            {
                return false;
            }

            _context.TiposAnimal.Remove(tipo);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void Aplicar(TipoAnimalEntity tipo, SalvarTipoAnimalRequest request)
        {
            tipo.Especie = request.Especie!.Value;
            tipo.Porte = request.Porte!.Value;
            tipo.Raca = request.Raca.Trim();
        }

        private static TipoAnimalDto ToDto(TipoAnimalEntity tipo)
        {
            return new TipoAnimalDto
            {
                Id = tipo.Id,
                Especie = tipo.Especie,
                Raca = tipo.Raca,
                Porte = tipo.Porte
            };
        }
    }
}
