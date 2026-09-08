using PetBuddies_API.Application.Dtos.Veterinario;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class VeterinarioService : IVeterinarioService
    {
        private readonly IVeterinarioRepository _repositorio;
        private readonly IUnitOfWork _unitOfWork;

        public VeterinarioService(IVeterinarioRepository repositorio, IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<VeterinarioDto>> ListarAsync()
        {
            var veterinarios = await _repositorio.ListarAsync();
            return veterinarios.Select(veterinario => veterinario.ToDto()).ToList();
        }

        public async Task<VeterinarioDto?> BuscarPorIdAsync(int veterinarioId)
        {
            var veterinario = await _repositorio.ObterPorIdAsync(veterinarioId);
            return veterinario?.ToDto();
        }

        public async Task<List<VeterinarioDto>> ListarPorClinicaAsync(int clinicaId)
        {
            var veterinarios = await _repositorio.ListarPorClinicaAsync(clinicaId);
            return veterinarios.Select(veterinario => veterinario.ToDto()).ToList();
        }

        public Task<bool> ClinicaExisteAsync(int clinicaId)
        {
            return _repositorio.ClinicaExisteAsync(clinicaId);
        }

        public Task<bool> CrmvExisteAsync(string crmv, int? ignorarVeterinarioId = null)
        {
            return _repositorio.CrmvExisteAsync(crmv, ignorarVeterinarioId);
        }

        public async Task<VeterinarioDto> CadastrarAsync(SalvarVeterinarioRequest request)
        {
            var veterinario = new VeterinarioEntity();
            veterinario.Aplicar(request);

            await _repositorio.AdicionarAsync(veterinario);
            await _unitOfWork.SalvarAsync();

            return veterinario.ToDto();
        }

        public async Task<VeterinarioDto?> AtualizarAsync(int veterinarioId, SalvarVeterinarioRequest request)
        {
            var veterinario = await _repositorio.ObterParaAlterarAsync(veterinarioId);

            if (veterinario is null)
            {
                return null;
            }

            veterinario.Aplicar(request);
            await _unitOfWork.SalvarAsync();

            return veterinario.ToDto();
        }

        public async Task<bool> RemoverAsync(int veterinarioId)
        {
            var veterinario = await _repositorio.ObterParaAlterarAsync(veterinarioId);

            if (veterinario is null)
            {
                return false;
            }

            _repositorio.Remover(veterinario);
            await _unitOfWork.SalvarAsync();

            return true;
        }
    }
}
