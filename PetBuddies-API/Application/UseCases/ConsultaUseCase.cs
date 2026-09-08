using PetBuddies_API.Application.Dtos.Consulta;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Infrastructure.Clients;

namespace PetBuddies_API.Application.UseCases
{
    public class ConsultaUseCase : IConsultaUseCase
    {
        private readonly IConsultaRepository _consultaRepositorio;
        private readonly IJanelaAtendimentoRepository _janelaAtendimentoRepositorio;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMotorApiClient _motorApiClient;

        public ConsultaUseCase(
            IConsultaRepository consultaRepositorio,
            IJanelaAtendimentoRepository janelaAtendimentoRepositorio,
            IUnitOfWork unitOfWork,
            IMotorApiClient motorApiClient)
        {
            _consultaRepositorio = consultaRepositorio;
            _janelaAtendimentoRepositorio = janelaAtendimentoRepositorio;
            _unitOfWork = unitOfWork;
            _motorApiClient = motorApiClient;
        }

        public Task<bool> AnimalExisteAsync(int animalId)
        {
            return _consultaRepositorio.AnimalExisteAsync(animalId);
        }

        public Task<bool> JanelaExisteAsync(int janelaId)
        {
            return _janelaAtendimentoRepositorio.ExisteAsync(janelaId);
        }

        public async Task<bool> JanelaOcupadaAsync(int janelaId, int? ignorarConsultaId = null)
        {
            var janela = await _janelaAtendimentoRepositorio.ObterParaVerificacaoAsync(janelaId);

            if (janela is null)
            {
                return false;
            }

            return await _consultaRepositorio.ExisteAtivaAsync(janela.VeterinarioId, janela.DataHoraInicio, ignorarConsultaId);
        }

        public Task<bool> ConsultaRealizadaAsync(int consultaId)
        {
            return _consultaRepositorio.ExisteRealizadaAsync(consultaId);
        }

        public async Task<ConsultaDto> AgendarAsync(AgendarConsultaRequest request)
        {
            var janela = await BuscarJanelaAsync(request.JanelaId);
            var dataHora = janela!.DataHoraInicio;

            var consulta = new ConsultaEntity
            {
                AnimalId = request.AnimalId,
                TipoConsulta = request.TipoConsulta!.Value,
                DataHora = dataHora,
                Status = StatusConsultaEnum.AGENDADA,
                VeterinarioId = janela.VeterinarioId
            };

            await _consultaRepositorio.AdicionarAsync(consulta);
            await _unitOfWork.SalvarAsync();

            return consulta.ToDto();
        }

        public async Task<List<ConsultaDto>> ListarAsync()
        {
            var consultas = await _consultaRepositorio.ListarAsync();
            return consultas.Select(consulta => consulta.ToDto()).ToList();
        }

        public async Task<List<ConsultaDto>> ListarPorAnimalAsync(int animalId)
        {
            var consultas = await _consultaRepositorio.ListarPorAnimalAsync(animalId);
            return consultas.Select(consulta => consulta.ToDto()).ToList();
        }

        public async Task<ConsultaDto?> BuscarPorIdAsync(int consultaId)
        {
            var consulta = await _consultaRepositorio.ObterPorIdAsync(consultaId);
            return consulta?.ToDto();
        }

        public async Task<ConsultaDto?> CancelarAsync(int consultaId, AtualizarConsultaStatusRequest request)
        {
            var consulta = await _consultaRepositorio.ObterParaAlterarAsync(consultaId);

            if (consulta is null)
            {
                return null;
            }

            consulta.Status = request.Status!.Value;
            consulta.Motivo = request.Motivo;

            await _unitOfWork.SalvarAsync();

            return consulta.ToDto();
        }

        public async Task<ConsultaDto?> AtualizarAsync(int consultaId, AtualizarConsultaRequest request)
        {
            var consulta = await _consultaRepositorio.ObterParaAlterarAsync(consultaId);

            if (consulta is null)
            {
                return null;
            }

            var statusAnterior = consulta.Status;

            var janela = await BuscarJanelaAsync(request.JanelaId);
            var dataHora = janela!.DataHoraInicio;

            consulta.AnimalId = request.AnimalId;
            consulta.TipoConsulta = request.TipoConsulta!.Value;
            consulta.DataHora = dataHora;
            consulta.Status = request.Status!.Value;
            consulta.Observacao = request.Observacao;
            consulta.VeterinarioId = janela.VeterinarioId;

            await _unitOfWork.SalvarAsync();

            return consulta.ToDto();
        }

        public async Task<bool> RemoverAsync(int consultaId)
        {
            var consulta = await _consultaRepositorio.ObterParaAlterarAsync(consultaId);

            if (consulta is null)
            {
                return false;
            }

            _consultaRepositorio.Remover(consulta);
            await _unitOfWork.SalvarAsync();

            return true;
        }

        private Task<JanelaAtendimentoEntity?> BuscarJanelaAsync(int janelaId)
        {
            return _janelaAtendimentoRepositorio.ObterParaVerificacaoAsync(janelaId);
        }
    }
}
