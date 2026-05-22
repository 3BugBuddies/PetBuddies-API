using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Prontuario;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class ProntuarioService
    {
        private readonly ApplicationContext _context;

        public ProntuarioService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<ProntuarioDto>> ListarAsync(int? animalId = null)
        {
            var query = _context.Prontuarios.AsNoTracking();

            if (animalId.HasValue)
            {
                query = query.Where(prontuario => prontuario.AnimalId == animalId.Value);
            }

            var prontuarios = await query
                .OrderBy(prontuario => prontuario.Id)
                .ToListAsync();

            return prontuarios.Select(ToDto).ToList();
        }

        public async Task<ProntuarioDto?> BuscarPorIdAsync(int prontuarioId)
        {
            var prontuario = await _context.Prontuarios
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == prontuarioId);

            return prontuario is null ? null : ToDto(prontuario);
        }

        public async Task<bool> AnimalExisteAsync(int animalId)
        {
            return await _context.Animais
                .AsNoTracking()
                .AnyAsync(item => item.Id == animalId);
        }

        public async Task<ProntuarioDto> CadastrarAsync(SalvarProntuarioRequest request)
        {
            var prontuario = new ProntuarioEntity();
            Aplicar(prontuario, request);

            _context.Prontuarios.Add(prontuario);
            await _context.SaveChangesAsync();

            return ToDto(prontuario);
        }

        public async Task<ProntuarioDto?> AtualizarAsync(int prontuarioId, SalvarProntuarioRequest request)
        {
            var prontuario = await _context.Prontuarios.SingleOrDefaultAsync(item => item.Id == prontuarioId);

            if (prontuario is null)
            {
                return null;
            }

            Aplicar(prontuario, request);
            await _context.SaveChangesAsync();

            return ToDto(prontuario);
        }

        public async Task<bool> RemoverAsync(int prontuarioId)
        {
            var prontuario = await _context.Prontuarios.SingleOrDefaultAsync(item => item.Id == prontuarioId);

            if (prontuario is null)
            {
                return false;
            }

            _context.Prontuarios.Remove(prontuario);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void Aplicar(ProntuarioEntity prontuario, SalvarProntuarioRequest request)
        {
            prontuario.Alergias = string.IsNullOrWhiteSpace(request.Alergias) ? null : request.Alergias.Trim();
            prontuario.Observacoes = string.IsNullOrWhiteSpace(request.Observacoes) ? null : request.Observacoes.Trim();
            prontuario.AnimalId = request.AnimalId;
        }

        private static ProntuarioDto ToDto(ProntuarioEntity prontuario)
        {
            return new ProntuarioDto
            {
                Id = prontuario.Id,
                Alergias = prontuario.Alergias,
                Observacoes = prontuario.Observacoes,
                AnimalId = prontuario.AnimalId
            };
        }
    }
}
