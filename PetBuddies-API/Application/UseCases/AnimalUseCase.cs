using PetBuddies_API.Application.Dtos.Animal;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Clients;

namespace PetBuddies_API.Application.UseCases
{
    public class AnimalUseCase : IAnimalUseCase
    {
        private readonly IAnimalRepository _repositorio;
        private readonly IMotorApiClient _motorApiClient;
        private readonly IUnitOfWork _unitOfWork;

        public AnimalUseCase(IAnimalRepository repositorio, IMotorApiClient motorApiClient, IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _motorApiClient = motorApiClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AnimalDto>> ListarAsync()
        {
            var animais = await _repositorio.ListarAsync();
            return animais.Select(animal => animal.ToDto()).ToList();
        }

        public async Task<AnimalDto?> BuscarPorIdAsync(int animalId)
        {
            var animal = await _repositorio.ObterPorIdAsync(animalId);
            return animal?.ToDto();
        }

        public Task<bool> ExisteAsync(int animalId)
        {
            return _repositorio.ExisteAsync(animalId);
        }

        public Task<bool> ResponsavelExisteAsync(int responsavelId)
        {
            return _repositorio.ResponsavelExisteAsync(responsavelId);
        }

        public Task<bool> PossuiConsultasAsync(int animalId)
        {
            return _repositorio.PossuiConsultasAsync(animalId);
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

            await _repositorio.AdicionarAsync(animal);
            await _unitOfWork.SalvarAsync();

            // Best-effort: falha do serviço de cuidado não desfaz o cadastro clínico.
            await _motorApiClient.InstanciarPlanoPreventivoAsync(
                animal.Id,
                animal.Especie,
                animal.Porte,
                animal.Sexo,
                animal.Castrado,
                animal.DataNascimento);

            return animal.ToDto();
        }

        public async Task<AnimalDto?> AtualizarAsync(int animalId, AtualizarAnimalRequest request)
        {
            var animal = await _repositorio.ObterParaAlterarAsync(animalId);

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

            await _unitOfWork.SalvarAsync();

            return animal.ToDto();
        }

        public async Task<bool> RemoverAsync(int animalId)
        {
            var animal = await _repositorio.ObterParaAlterarAsync(animalId);

            if (animal is null)
            {
                return false;
            }

            _repositorio.Remover(animal);
            await _unitOfWork.SalvarAsync();

            return true;
        }
    }
}
