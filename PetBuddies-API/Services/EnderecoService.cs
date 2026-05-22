using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Endereco;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class EnderecoService
    {
        private readonly ApplicationContext _context;

        public EnderecoService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<EnderecoDto>> ListarAsync()
        {
            var enderecos = await _context.Enderecos
                .AsNoTracking()
                .OrderBy(endereco => endereco.Id)
                .ToListAsync();

            return enderecos.Select(ToDto).ToList();
        }

        public async Task<EnderecoDto?> BuscarPorIdAsync(int enderecoId)
        {
            var endereco = await _context.Enderecos
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == enderecoId);

            return endereco is null ? null : ToDto(endereco);
        }

        public async Task<bool> ExisteAsync(int enderecoId)
        {
            return await _context.Enderecos
                .AsNoTracking()
                .AnyAsync(item => item.Id == enderecoId);
        }

        public async Task<EnderecoDto> CadastrarAsync(SalvarEnderecoRequest request)
        {
            var endereco = new EnderecoEntity();
            Aplicar(endereco, request);

            _context.Enderecos.Add(endereco);
            await _context.SaveChangesAsync();

            return ToDto(endereco);
        }

        public async Task<EnderecoDto?> AtualizarAsync(int enderecoId, SalvarEnderecoRequest request)
        {
            var endereco = await _context.Enderecos.SingleOrDefaultAsync(item => item.Id == enderecoId);

            if (endereco is null)
            {
                return null;
            }

            Aplicar(endereco, request);
            await _context.SaveChangesAsync();

            return ToDto(endereco);
        }

        public async Task<bool> RemoverAsync(int enderecoId)
        {
            var endereco = await _context.Enderecos.SingleOrDefaultAsync(item => item.Id == enderecoId);

            if (endereco is null)
            {
                return false;
            }

            _context.Enderecos.Remove(endereco);
            await _context.SaveChangesAsync();

            return true;
        }

        private static void Aplicar(EnderecoEntity endereco, SalvarEnderecoRequest request)
        {
            endereco.Logradouro = request.Logradouro.Trim();
            endereco.Numero = request.Numero.Trim();
            endereco.Complemento = string.IsNullOrWhiteSpace(request.Complemento) ? null : request.Complemento.Trim();
            endereco.Bairro = string.IsNullOrWhiteSpace(request.Bairro) ? null : request.Bairro.Trim();
            endereco.Cidade = request.Cidade.Trim();
            endereco.Estado = request.Estado.Trim().ToUpperInvariant();
            endereco.Cep = request.Cep.Trim();
        }

        private static EnderecoDto ToDto(EnderecoEntity endereco)
        {
            return new EnderecoDto
            {
                Id = endereco.Id,
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep
            };
        }
    }
}
