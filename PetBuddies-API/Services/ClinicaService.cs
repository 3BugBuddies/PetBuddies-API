using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Clinica;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class ClinicaService
    {
        private readonly ApplicationContext _context;

        public ClinicaService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<ClinicaDto>> ListarAsync()
        {
            var clinicas = await _context.Clinicas
                .AsNoTracking()
                .OrderBy(clinica => clinica.Id)
                .ToListAsync();

            return clinicas.Select(ToDto).ToList();
        }

        public async Task<ClinicaDto?> BuscarPorIdAsync(int clinicaId)
        {
            var clinica = await _context.Clinicas
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == clinicaId);

            return clinica is null ? null : ToDto(clinica);
        }

        public async Task<List<ClinicaDto>> BuscarPorNomeAsync(string nome)
        {
            var clinicas = await _context.Clinicas
                .AsNoTracking()
                .Where(item => item.Nome.ToLower().Contains(nome.ToLower()))
                .OrderBy(item => item.Nome)
                .ToListAsync();

            return clinicas.Select(ToDto).ToList();
        }

        public async Task<bool> EnderecoExisteAsync(int enderecoId)
        {
            return await _context.Enderecos
                .AsNoTracking()
                .AnyAsync(item => item.Id == enderecoId);
        }

        public async Task<bool> CnpjExisteAsync(string cnpj, int? ignorarClinicaId = null)
        {
            var cnpjNormalizado = cnpj.Trim();

            var query = _context.Clinicas
                .AsNoTracking()
                .Where(item => item.Cnpj == cnpjNormalizado);

            if (ignorarClinicaId.HasValue)
                query = query.Where(item => item.Id != ignorarClinicaId.Value);

            return await query.AnyAsync();
        }

        public async Task<ClinicaDto> CadastrarAsync(SalvarClinicaRequest request)
        {
            var clinica = new ClinicaEntity();
            Aplicar(clinica, request);

            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return ToDto(clinica);
        }

        public async Task<ClinicaDto?> AtualizarAsync(int clinicaId, SalvarClinicaRequest request)
        {
            var clinica = await _context.Clinicas.SingleOrDefaultAsync(item => item.Id == clinicaId);

            if (clinica is null)
            {
                return null;
            }

            Aplicar(clinica, request);
            await _context.SaveChangesAsync();

            return ToDto(clinica);
        }

        public async Task<bool> RemoverAsync(int clinicaId)
        {
            var clinica = await _context.Clinicas.SingleOrDefaultAsync(item => item.Id == clinicaId);

            if (clinica is null)
            {
                return false;
            }

            _context.Clinicas.Remove(clinica);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void Aplicar(ClinicaEntity clinica, SalvarClinicaRequest request)
        {
            clinica.Nome = request.Nome.Trim();
            clinica.Cnpj = request.Cnpj.Trim();
            clinica.Telefone = request.Telefone.Trim();
            clinica.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            clinica.EnderecoId = request.EnderecoId;
        }

        private static ClinicaDto ToDto(ClinicaEntity clinica)
        {
            return new ClinicaDto
            {
                Id = clinica.Id,
                Nome = clinica.Nome,
                Cnpj = clinica.Cnpj,
                Telefone = clinica.Telefone,
                Email = clinica.Email,
                EnderecoId = clinica.EnderecoId
            };
        }
    }
}
