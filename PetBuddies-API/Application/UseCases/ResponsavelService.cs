using PetBuddies_API.Application.Dtos.Animal;
using PetBuddies_API.Application.Dtos.Responsavel;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class ResponsavelService : IResponsavelService
    {
        private readonly IResponsavelRepository _repositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ResponsavelService(IResponsavelRepository repositorio, IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponsavelDto?> BuscarPorTelefoneAsync(string telefone)
        {
            var telefoneNormalizado = NormalizarTelefone(telefone);
            var responsavel = await _repositorio.ObterPorTelefoneAsync(telefoneNormalizado);

            return responsavel?.ToDto();
        }

        public async Task<List<ResponsavelDto>> ListarAsync()
        {
            var responsaveis = await _repositorio.ListarAsync();
            return responsaveis.Select(responsavel => responsavel.ToDto()).ToList();
        }

        public async Task<ResponsavelDto?> BuscarPorIdAsync(int responsavelId)
        {
            var responsavel = await _repositorio.ObterPorIdAsync(responsavelId);
            return responsavel?.ToDto();
        }

        public Task<bool> TelefoneExisteAsync(string telefone, int? ignorarResponsavelId = null)
        {
            var telefoneNormalizado = NormalizarTelefone(telefone);
            return _repositorio.TelefoneExisteAsync(telefoneNormalizado, ignorarResponsavelId);
        }

        public Task<bool> PossuiAnimaisAsync(int responsavelId)
        {
            return _repositorio.PossuiAnimaisAsync(responsavelId);
        }

        public async Task<ResponsavelDto> CadastrarAsync(CadastrarResponsavelRequest request)
        {
            var responsavel = new ResponsavelEntity
            {
                Nome = request.Nome.Trim(),
                Telefone = NormalizarTelefone(request.Telefone),
                Email = null
            };

            await _repositorio.AdicionarAsync(responsavel);
            await _unitOfWork.SalvarAsync();

            return responsavel.ToDto();
        }

        public async Task<ResponsavelDto?> AtualizarAsync(int responsavelId, CadastrarResponsavelRequest request)
        {
            var responsavel = await _repositorio.ObterParaAlterarAsync(responsavelId);

            if (responsavel is null)
            {
                return null;
            }

            responsavel.Nome = request.Nome.Trim();
            responsavel.Telefone = NormalizarTelefone(request.Telefone);

            await _unitOfWork.SalvarAsync();

            return responsavel.ToDto();
        }

        public async Task<bool> RemoverAsync(int responsavelId)
        {
            var responsavel = await _repositorio.ObterParaAlterarAsync(responsavelId);

            if (responsavel is null)
            {
                return false;
            }

            _repositorio.Remover(responsavel);
            await _unitOfWork.SalvarAsync();

            return true;
        }

        public async Task<List<AnimalDto>?> ListarAnimaisAsync(int responsavelId)
        {
            var existeResponsavel = await _repositorio.ExisteAsync(responsavelId);

            if (!existeResponsavel)
            {
                return null;
            }

            var animais = await _repositorio.ListarAnimaisAsync(responsavelId);
            return animais.Select(animal => animal.ToDto()).ToList();
        }

        private static string NormalizarTelefone(string telefone)
        {
            return new string(telefone.Where(char.IsDigit).ToArray());
        }
    }
}
