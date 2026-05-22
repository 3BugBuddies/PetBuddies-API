using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Animal;
using PetBuddies_API.Dtos.Responsavel;
using PetBuddies_API.Enums;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class ResponsavelService
    {
        private const int ClinicaId = 1;

        private readonly ApplicationContext _context;

        public ResponsavelService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ResponsavelDto?> BuscarPorTelefoneAsync(string telefone)
        {
            var telefoneNormalizado = NormalizarTelefone(telefone);
            var responsavel = await _context.Responsaveis
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Telefone == telefoneNormalizado);

            return responsavel is null ? null : ToDto(responsavel);
        }

        public async Task<List<ResponsavelDto>> ListarAsync()
        {
            var responsaveis = await _context.Responsaveis
                .AsNoTracking()
                .OrderBy(item => item.Id)
                .ToListAsync();

            return responsaveis.Select(ToDto).ToList();
        }

        public async Task<ResponsavelDto?> BuscarPorIdAsync(int responsavelId)
        {
            var responsavel = await _context.Responsaveis
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == responsavelId);

            return responsavel is null ? null : ToDto(responsavel);
        }

        public async Task<bool> TelefoneExisteAsync(string telefone, int? ignorarResponsavelId = null)
        {
            var telefoneNormalizado = NormalizarTelefone(telefone);
            var query = _context.Responsaveis
                .AsNoTracking()
                .Where(item => item.Telefone == telefoneNormalizado);

            if (ignorarResponsavelId.HasValue)
                query = query.Where(item => item.Id != ignorarResponsavelId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> PossuiAnimaisAsync(int responsavelId)
        {
            return await _context.Animais
                .AsNoTracking()
                .AnyAsync(animal => animal.ResponsavelId == responsavelId);
        }

        public async Task<ResponsavelDto> CadastrarAsync(CadastrarResponsavelRequest request)
        {
            var responsavel = new ResponsavelEntity
            {
                Nome = request.Nome.Trim(),
                Cpf = null,
                Telefone = NormalizarTelefone(request.Telefone),
                Email = null,
                Status = StatusTutorEnum.PRE_CADASTRO,
                ClinicaId = ClinicaId,
                EnderecoId = null
            };

            _context.Responsaveis.Add(responsavel);
            await _context.SaveChangesAsync();

            return ToDto(responsavel);
        }

        public async Task<ResponsavelDto?> AtualizarAsync(int responsavelId, CadastrarResponsavelRequest request)
        {
            var responsavel = await _context.Responsaveis
                .SingleOrDefaultAsync(item => item.Id == responsavelId);

            if (responsavel is null)
            {
                return null;
            }

            responsavel.Nome = request.Nome.Trim();
            responsavel.Telefone = NormalizarTelefone(request.Telefone);
            responsavel.ClinicaId = ClinicaId;

            await _context.SaveChangesAsync();

            return ToDto(responsavel);
        }

        public async Task<bool> RemoverAsync(int responsavelId)
        {
            var responsavel = await _context.Responsaveis
                .SingleOrDefaultAsync(item => item.Id == responsavelId);

            if (responsavel is null)
            {
                return false;
            }

            _context.Responsaveis.Remove(responsavel);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<AnimalDto>?> ListarAnimaisAsync(int responsavelId)
        {
            var existeResponsavel = await _context.Responsaveis
                .AsNoTracking()
                .AnyAsync(item => item.Id == responsavelId);

            if (!existeResponsavel)
            {
                return null;
            }

            return await _context.Animais
                .AsNoTracking()
                .Include(animal => animal.TipoAnimal)
                .Where(animal => animal.ResponsavelId == responsavelId)
                .Select(animal => new AnimalDto
                {
                    Id = animal.Id,
                    Nome = animal.Nome,
                    Especie = animal.TipoAnimal != null ? animal.TipoAnimal.Especie.ToString() : string.Empty,
                    Porte = animal.TipoAnimal != null ? animal.TipoAnimal.Porte.ToString() : string.Empty,
                    Sexo = animal.Sexo.ToString(),
                    Castrado = animal.Castrado,
                    PreCadastro = animal.PreCadastro
                })
                .ToListAsync();
        }

        private static ResponsavelDto ToDto(ResponsavelEntity responsavel)
        {
            return new ResponsavelDto
            {
                Id = responsavel.Id,
                Nome = responsavel.Nome,
                Telefone = responsavel.Telefone,
                Status = responsavel.Status.ToString()
            };
        }

        private static string NormalizarTelefone(string telefone)
        {
            return new string(telefone.Where(char.IsDigit).ToArray());
        }
    }
}
