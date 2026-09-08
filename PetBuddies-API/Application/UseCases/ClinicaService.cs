using PetBuddies_API.Application.Dtos.Clinica;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class ClinicaService : IClinicaService
    {
        private readonly IClinicaRepository _repositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ClinicaService(IClinicaRepository repositorio, IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ClinicaDto>> ListarAsync()
        {
            var clinicas = await _repositorio.ListarAsync();
            return clinicas.Select(clinica => clinica.ToDto()).ToList();
        }

        public async Task<ClinicaDto?> BuscarPorIdAsync(int clinicaId)
        {
            var clinica = await _repositorio.ObterPorIdAsync(clinicaId);
            return clinica?.ToDto();
        }

        public async Task<List<ClinicaDto>> BuscarPorNomeAsync(string nome)
        {
            var clinicas = await _repositorio.BuscarPorNomeAsync(nome);
            return clinicas.Select(clinica => clinica.ToDto()).ToList();
        }

        public Task<bool> CnpjExisteAsync(string cnpj, int? ignorarClinicaId = null)
        {
            return _repositorio.CnpjExisteAsync(cnpj, ignorarClinicaId);
        }

        public async Task<ClinicaDto> CadastrarAsync(SalvarClinicaRequest request)
        {
            var clinica = new ClinicaEntity();
            clinica.Aplicar(request);

            await _repositorio.AdicionarAsync(clinica);
            await _unitOfWork.SalvarAsync();

            return clinica.ToDto();
        }

        public async Task<ClinicaDto?> AtualizarAsync(int clinicaId, SalvarClinicaRequest request)
        {
            var clinica = await _repositorio.ObterParaAlterarAsync(clinicaId);

            if (clinica is null)
            {
                return null;
            }

            clinica.Aplicar(request);
            await _unitOfWork.SalvarAsync();

            return clinica.ToDto();
        }

        public async Task<bool> RemoverAsync(int clinicaId)
        {
            var clinica = await _repositorio.ObterParaAlterarAsync(clinicaId);

            if (clinica is null)
            {
                return false;
            }

            _repositorio.Remover(clinica);
            await _unitOfWork.SalvarAsync();

            return true;
        }
    }
}
