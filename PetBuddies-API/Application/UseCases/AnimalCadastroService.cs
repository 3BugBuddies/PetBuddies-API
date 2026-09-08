using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Infrastructure.Clients;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.Dtos.Animal;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.UseCases
{
    public class AnimalCadastroService
    {
        private readonly ApplicationContext _context;
        private readonly MotorApiClient _motorApiClient;

        public AnimalCadastroService(ApplicationContext context, MotorApiClient motorApiClient)
        {
            _context = context;
            _motorApiClient = motorApiClient;
        }

        public async Task<List<AnimalDto>> ListarAsync()
        {
            var animais = await _context.Animais
                .AsNoTracking()
                .OrderBy(animal => animal.Id)
                .ToListAsync();

            return animais.Select(ToDto).ToList();
        }

        public async Task<AnimalDto?> BuscarPorIdAsync(int animalId)
        {
            var animal = await _context.Animais
                .AsNoTracking()
                .SingleOrDefaultAsync(animal => animal.Id == animalId);

            return animal is null ? null : ToDto(animal);
        }

        public async Task<bool> ExisteAsync(int animalId)
        {
            return await _context.Animais
                .AsNoTracking()
                .AnyAsync(animal => animal.Id == animalId);
        }

        public async Task<bool> ResponsavelExisteAsync(int responsavelId)
        {
            return await _context.Responsaveis
                .AsNoTracking()
                .AnyAsync(responsavel => responsavel.Id == responsavelId);
        }

        public async Task<bool> PossuiConsultasAsync(int animalId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(consulta => consulta.AnimalId == animalId);
        }

        public async Task<AnimalDto> CadastrarAsync(CadastrarAnimalRequest request)
        {
            var animal = new AnimalEntity
            {
                ResponsavelId = request.ResponsavelId,
                Nome = request.Nome.Trim(),
                Especie = request.Especie!.Value,
                Raca = string.IsNullOrWhiteSpace(request.Raca) ? "SEM_RACA" : request.Raca.Trim(),
                Porte = request.Porte!.Value,
                Sexo = request.Sexo!.Value,
                DataNascimento = request.DataNascimento!.Value,
                // Peso fixo em zero desde a Sprint 2. A spec do N1 mantem assim:
                // quem passa a gravar peso de verdade e o fechamento de atendimento, no N7.
                Peso = 0,
                CondicaoCronica = false,
                Castrado = request.Castrado,
                Foto = null,
                Alergias = request.Alergias,
                Observacoes = request.Observacoes
            };

            _context.Animais.Add(animal);
            await _context.SaveChangesAsync();

            // Best-effort: falha do serviço de cuidado não desfaz o cadastro clínico.
            await _motorApiClient.InstanciarPlanoPreventivoAsync(
                animal.Id,
                animal.Especie,
                animal.Porte,
                animal.Sexo,
                animal.Castrado,
                animal.DataNascimento);

            return ToDto(animal);
        }

        public async Task<AnimalDto?> AtualizarAsync(int animalId, AtualizarAnimalRequest request)
        {
            var animal = await _context.Animais
                .SingleOrDefaultAsync(item => item.Id == animalId);

            if (animal is null)
            {
                return null;
            }

            animal.ResponsavelId = request.ResponsavelId;
            animal.Nome = request.Nome.Trim();
            animal.Especie = request.Especie!.Value;
            animal.Raca = string.IsNullOrWhiteSpace(request.Raca) ? "SEM_RACA" : request.Raca.Trim();
            animal.Porte = request.Porte!.Value;
            animal.Sexo = request.Sexo!.Value;
            animal.DataNascimento = request.DataNascimento!.Value;
            animal.CondicaoCronica = request.CondicaoCronica;
            animal.Castrado = request.Castrado;
            animal.Alergias = request.Alergias;
            animal.Observacoes = request.Observacoes;

            await _context.SaveChangesAsync();

            return ToDto(animal);
        }

        public async Task<bool> RemoverAsync(int animalId)
        {
            var animal = await _context.Animais.SingleOrDefaultAsync(item => item.Id == animalId);

            if (animal is null)
            {
                return false;
            }

            _context.Animais.Remove(animal);
            await _context.SaveChangesAsync();

            return true;
        }

        private static AnimalDto ToDto(AnimalEntity animal)
        {
            return new AnimalDto
            {
                Id = animal.Id,
                Nome = animal.Nome,
                Especie = animal.Especie.ToString(),
                Raca = animal.Raca,
                Porte = animal.Porte.ToString(),
                Sexo = animal.Sexo.ToString(),
                Castrado = animal.Castrado,
                CondicaoCronica = animal.CondicaoCronica,
                DataNascimento = animal.DataNascimento,
                Peso = animal.Peso,
                Alergias = animal.Alergias,
                Observacoes = animal.Observacoes
            };
        }
    }
}
