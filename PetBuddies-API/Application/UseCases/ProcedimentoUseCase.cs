using PetBuddies_API.Application.Dtos.Procedimento;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Clients;

namespace PetBuddies_API.Application.UseCases
{
    public class ProcedimentoUseCase : IProcedimentoUseCase
    {
        private readonly IProcedimentoRepository _procedimentoRepositorio;
        private readonly IRegistroAtendimentoRepository _registroAtendimentoRepositorio;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMotorApiClient _motorApiClient;

        public ProcedimentoUseCase(
            IProcedimentoRepository procedimentoRepositorio,
            IRegistroAtendimentoRepository registroAtendimentoRepositorio,
            IUnitOfWork unitOfWork,
            IMotorApiClient motorApiClient)
        {
            _procedimentoRepositorio = procedimentoRepositorio;
            _registroAtendimentoRepositorio = registroAtendimentoRepositorio;
            _unitOfWork = unitOfWork;
            _motorApiClient = motorApiClient;
        }

        public async Task<List<ProcedimentoDto>> ListarAsync(int? animalId = null)
        {
            var procedimentos = await _procedimentoRepositorio.ListarAsync(animalId);
            return procedimentos.Select(procedimento => procedimento.ToDto()).ToList();
        }

        public async Task<ProcedimentoDto?> BuscarPorIdAsync(int procedimentoId)
        {
            var procedimento = await _procedimentoRepositorio.ObterPorIdAsync(procedimentoId);
            return procedimento?.ToDto();
        }

        public Task<bool> AnimalExisteAsync(int animalId)
        {
            return _procedimentoRepositorio.AnimalExisteAsync(animalId);
        }

        public Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return _procedimentoRepositorio.VeterinarioExisteAsync(veterinarioId);
        }

        public Task<bool> RegistroAtendimentoPertenceAoAnimalAsync(int registroAtendimentoId, int animalId)
        {
            return _registroAtendimentoRepositorio.PertenceAoAnimalAsync(registroAtendimentoId, animalId);
        }

        public async Task<ProcedimentoDto> CadastrarAsync(SalvarProcedimentoRequest request)
        {
            var procedimento = new ProcedimentoEntity();
            procedimento.Aplicar(request);

            await _procedimentoRepositorio.AdicionarAsync(procedimento);
            await _unitOfWork.SalvarAsync();

            await DispararPlanoPosCirurgicoSeNecessarioAsync(procedimento);

            return procedimento.ToDto();
        }

        public async Task<ProcedimentoDto?> AtualizarAsync(int procedimentoId, SalvarProcedimentoRequest request)
        {
            var procedimento = await _procedimentoRepositorio.ObterParaAlterarAsync(procedimentoId);

            if (procedimento is null)
            {
                return null;
            }

            var jaEraCirurgiaRealizada = EhCirurgiaRealizada(procedimento);
            procedimento.Aplicar(request);

            await _unitOfWork.SalvarAsync();

            if (!jaEraCirurgiaRealizada)
            {
                await DispararPlanoPosCirurgicoSeNecessarioAsync(procedimento);
            }

            return procedimento.ToDto();
        }

        public async Task<bool> RemoverAsync(int procedimentoId)
        {
            var procedimento = await _procedimentoRepositorio.ObterParaAlterarAsync(procedimentoId);

            if (procedimento is null)
            {
                return false;
            }

            _procedimentoRepositorio.Remover(procedimento);
            await _unitOfWork.SalvarAsync();

            return true;
        }

        private async Task DispararPlanoPosCirurgicoSeNecessarioAsync(ProcedimentoEntity procedimento)
        {
            if (!EhCirurgiaRealizada(procedimento))
            {
                return;
            }

            var consultaId = await _registroAtendimentoRepositorio.ObterConsultaIdAsync(procedimento.RegistroAtendimentoId);

            await _motorApiClient.InstanciarPlanoPosCirurgicoAsync(procedimento.AnimalId, consultaId);
        }

        private static bool EhCirurgiaRealizada(ProcedimentoEntity procedimento)
        {
            return procedimento.Tipo == TipoProcedimentoEnum.CIRURGIA
                && procedimento.Status == StatusProcedimentoEnum.REALIZADO;
        }
    }
}
