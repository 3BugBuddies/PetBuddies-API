using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Veterinario;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class VeterinarioService
    {
        private readonly ApplicationContext _context;

        public VeterinarioService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<VeterinarioDto>> ListarAsync()
        {
            var veterinarios = await _context.Veterinarios
                .AsNoTracking()
                .OrderBy(veterinario => veterinario.Id)
                .ToListAsync();

            return veterinarios.Select(ToDto).ToList();
        }

        public async Task<VeterinarioDto?> BuscarPorIdAsync(int veterinarioId)
        {
            var veterinario = await _context.Veterinarios
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == veterinarioId);

            return veterinario is null ? null : ToDto(veterinario);
        }

        public async Task<List<VeterinarioDto>> ListarPorClinicaAsync(int clinicaId)
        {
            var veterinarios = await _context.Veterinarios
                .AsNoTracking()
                .Where(item => item.ClinicaId == clinicaId)
                .OrderBy(item => item.Nome)
                .ToListAsync();

            return veterinarios.Select(ToDto).ToList();
        }

        public async Task<bool> ClinicaExisteAsync(int clinicaId)
        {
            return await _context.Clinicas
                .AsNoTracking()
                .AnyAsync(item => item.Id == clinicaId);
        }

        public async Task<bool> CrmvExisteAsync(string crmv, int clinicaId, int? ignorarVeterinarioId = null)
        {
            var crmvNormalizado = crmv.Trim();

            var query = _context.Veterinarios
                .AsNoTracking()
                .Where(item => item.Crmv == crmvNormalizado && item.ClinicaId == clinicaId);

            if (ignorarVeterinarioId.HasValue)
                query = query.Where(item => item.Id != ignorarVeterinarioId.Value);

            return await query.AnyAsync();
        }

        public async Task<VeterinarioDto> CadastrarAsync(SalvarVeterinarioRequest request)
        {
            var veterinario = new VeterinarioEntity();
            Aplicar(veterinario, request);

            _context.Veterinarios.Add(veterinario);
            await _context.SaveChangesAsync();

            return ToDto(veterinario);
        }

        public async Task<VeterinarioDto?> AtualizarAsync(int veterinarioId, SalvarVeterinarioRequest request)
        {
            var veterinario = await _context.Veterinarios.SingleOrDefaultAsync(item => item.Id == veterinarioId);

            if (veterinario is null)
            {
                return null;
            }

            Aplicar(veterinario, request);
            await _context.SaveChangesAsync();

            return ToDto(veterinario);
        }

        public async Task<bool> RemoverAsync(int veterinarioId)
        {
            var veterinario = await _context.Veterinarios.SingleOrDefaultAsync(item => item.Id == veterinarioId);

            if (veterinario is null)
            {
                return false;
            }

            _context.Veterinarios.Remove(veterinario);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void Aplicar(VeterinarioEntity veterinario, SalvarVeterinarioRequest request)
        {
            veterinario.Nome = request.Nome.Trim();
            veterinario.Crmv = request.Crmv.Trim();
            veterinario.Especialidade = string.IsNullOrWhiteSpace(request.Especialidade) ? null : request.Especialidade.Trim();
            veterinario.Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone.Trim();
            veterinario.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            veterinario.AtendeEmergencia = request.AtendeEmergencia;
            veterinario.Ativo = request.Ativo;
            veterinario.ClinicaId = request.ClinicaId;
        }

        private static VeterinarioDto ToDto(VeterinarioEntity veterinario)
        {
            return new VeterinarioDto
            {
                Id = veterinario.Id,
                Nome = veterinario.Nome,
                Crmv = veterinario.Crmv,
                Especialidade = veterinario.Especialidade,
                Telefone = veterinario.Telefone,
                Email = veterinario.Email,
                AtendeEmergencia = veterinario.AtendeEmergencia,
                Ativo = veterinario.Ativo,
                ClinicaId = veterinario.ClinicaId
            };
        }
    }
}
