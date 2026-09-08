using PetBuddies_API.Application.Dtos.RegistroAtendimento;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class RegistroAtendimentoUseCase : IRegistroAtendimentoUseCase
    {
        private readonly IRegistroAtendimentoRepository _registroAtendimentoRepositorio;
        private readonly IConsultaRepository _consultaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public RegistroAtendimentoUseCase(
            IRegistroAtendimentoRepository registroAtendimentoRepositorio,
            IConsultaRepository consultaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _registroAtendimentoRepositorio = registroAtendimentoRepositorio;
            _consultaRepositorio = consultaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RegistroAtendimentoDto>> ListarAsync(int? animalId = null)
        {
            var registros = await _registroAtendimentoRepositorio.ListarAsync(animalId);
            return registros.Select(registro => registro.ToDto()).ToList();
        }

        public async Task<RegistroAtendimentoDto?> BuscarPorIdAsync(int registroAtendimentoId)
        {
            var registro = await _registroAtendimentoRepositorio.ObterPorIdAsync(registroAtendimentoId);
            return registro?.ToDto();
        }

        public Task<bool> AnimalExisteAsync(int animalId)
        {
            return _registroAtendimentoRepositorio.AnimalExisteAsync(animalId);
        }

        public Task<bool> ConsultaPertenceAoAnimalAsync(int consultaId, int animalId)
        {
            return _consultaRepositorio.PertenceAoAnimalAsync(consultaId, animalId);
        }

        public async Task<RegistroAtendimentoDto> CadastrarAsync(SalvarRegistroAtendimentoRequest request)
        {
            var registro = new RegistroAtendimentoEntity();
            registro.Aplicar(request);

            await _registroAtendimentoRepositorio.AdicionarAsync(registro);
            await _unitOfWork.SalvarAsync();

            return registro.ToDto();
        }

        public async Task<RegistroAtendimentoDto?> AtualizarAsync(int registroAtendimentoId, SalvarRegistroAtendimentoRequest request)
        {
            var registro = await _registroAtendimentoRepositorio.ObterParaAlterarAsync(registroAtendimentoId);

            if (registro is null)
            {
                return null;
            }

            registro.Aplicar(request);

            await _unitOfWork.SalvarAsync();

            return registro.ToDto();
        }

        public async Task<bool> RemoverAsync(int registroAtendimentoId)
        {
            var registro = await _registroAtendimentoRepositorio.ObterParaAlterarAsync(registroAtendimentoId);

            if (registro is null)
            {
                return false;
            }

            _registroAtendimentoRepositorio.Remover(registro);
            await _unitOfWork.SalvarAsync();

            return true;
        }
    }
}
