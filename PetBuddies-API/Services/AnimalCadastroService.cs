using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Animal;
using PetBuddies_API.Enums;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
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
                .Include(animal => animal.TipoAnimal)
                .OrderBy(animal => animal.Id)
                .ToListAsync();

            return animais.Select(ToDto).ToList();
        }

        public async Task<AnimalDto?> BuscarPorIdAsync(int animalId)
        {
            var animal = await _context.Animais
                .AsNoTracking()
                .Include(animal => animal.TipoAnimal)
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

        public async Task<bool> TipoAnimalExisteAsync(EspecieEnum especie, PorteEnum porte)
        {
            return await _context.TiposAnimal
                .AsNoTracking()
                .AnyAsync(item => item.Especie == especie && item.Porte == porte);
        }

        public async Task<bool> PossuiConsultasAsync(int animalId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(consulta => consulta.AnimalId == animalId);
        }

        public async Task<AnimalDto> CadastrarAsync(CadastrarAnimalRequest request)
        {
            var especie = request.Especie!.Value;
            var porte = request.Porte!.Value;
            var tipoAnimal = await BuscarTipoAnimalAsync(especie, porte);

            var animal = new AnimalEntity
            {
                ResponsavelId = request.ResponsavelId,
                Nome = request.Nome.Trim(),
                Sexo = request.Sexo!.Value,
                DataNascimento = request.DataNascimento!.Value,
                Peso = 0,
                CondicaoCronica = false,
                PreCadastro = true,
                Castrado = request.Castrado,
                Foto = null,
                TipoAnimalId = tipoAnimal!.Id
            };

            _context.Animais.Add(animal);
            await _context.SaveChangesAsync();

            await _motorApiClient.InstanciarPlanoPreventivoAsync(
                animal.Id,
                especie,
                porte,
                animal.Sexo,
                animal.Castrado,
            
                animal.DataNascimento);

            return ToDto(animal, especie, porte);
        }

        public async Task<AnimalDto?> AtualizarAsync(int animalId, AtualizarAnimalRequest request)
        {
            var animal = await _context.Animais
                .SingleOrDefaultAsync(item => item.Id == animalId);

            if (animal is null)
            {
                return null;
            }

            var especie = request.Especie!.Value;
            var porte = request.Porte!.Value;
            var tipoAnimal = await BuscarTipoAnimalAsync(especie, porte);

            var dataNascimentoOld = animal.DataNascimento;
            var castradoOld = animal.Castrado;
            var condicaoCronicaOld = animal.CondicaoCronica;
            var tipoAnimalIdOld = animal.TipoAnimalId;

            animal.ResponsavelId = request.ResponsavelId;
            animal.Nome = request.Nome.Trim();
            animal.Sexo = request.Sexo!.Value;
            animal.DataNascimento = request.DataNascimento!.Value;
            animal.CondicaoCronica = request.CondicaoCronica;
            animal.PreCadastro = request.PreCadastro;
            animal.Castrado = request.Castrado;
            animal.TipoAnimalId = tipoAnimal!.Id;

            await _context.SaveChangesAsync();

            bool mudouCampoClinco = dataNascimentoOld != animal.DataNascimento
                || castradoOld != animal.Castrado
                || condicaoCronicaOld != animal.CondicaoCronica
                || tipoAnimalIdOld != animal.TipoAnimalId;

            if (mudouCampoClinco)
                await _motorApiClient.RecalcularScoreAsync(animal.Id, "ATUALIZACAO_CLINICA");

            return ToDto(animal, especie, porte);
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

        private Task<TipoAnimalEntity?> BuscarTipoAnimalAsync(EspecieEnum especie, PorteEnum porte)
        {
            return _context.TiposAnimal
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Especie == especie && item.Porte == porte);
        }

        private static AnimalDto ToDto(AnimalEntity animal)
        {
            return new AnimalDto
            {
                Id = animal.Id,
                Nome = animal.Nome,
                Especie = animal.TipoAnimal?.Especie.ToString() ?? string.Empty,
                Porte = animal.TipoAnimal?.Porte.ToString() ?? string.Empty,
                Sexo = animal.Sexo.ToString(),
                Castrado = animal.Castrado,
                PreCadastro = animal.PreCadastro
            };
        }

        private static AnimalDto ToDto(AnimalEntity animal, EspecieEnum especie, PorteEnum porte)
        {
            return new AnimalDto
            {
                Id = animal.Id,
                Nome = animal.Nome,
                Especie = especie.ToString(),
                Porte = porte.ToString(),
                Sexo = animal.Sexo.ToString(),
                Castrado = animal.Castrado,
                PreCadastro = animal.PreCadastro
            };
        }
    }
}
