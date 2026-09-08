using PetBuddies_API.Application.Dtos.JanelaAtendimento;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class JanelaAtendimentoService : IJanelaAtendimentoService
    {
        private readonly IJanelaAtendimentoRepository _janelaAtendimentoRepositorio;
        private readonly IConsultaRepository _consultaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public JanelaAtendimentoService(
            IJanelaAtendimentoRepository janelaAtendimentoRepositorio,
            IConsultaRepository consultaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _janelaAtendimentoRepositorio = janelaAtendimentoRepositorio;
            _consultaRepositorio = consultaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<JanelaAtendimentoDto>> ListarDisponiveisAsync()
        {
            var agora = DateTime.Now;

            var ocupacoes = await _consultaRepositorio.ListarOcupacoesFuturasAsync(agora);
            var ocupadas = ocupacoes.ToHashSet();

            var janelas = await _janelaAtendimentoRepositorio.ListarFuturasAsync(agora);

            return janelas
                .Where(janela => !ocupadas.Contains((janela.VeterinarioId, janela.DataHoraInicio)))
                .Select(janela => janela.ToDto())
                .ToList();
        }

        public async Task<List<JanelaAtendimentoDto>> ListarAsync()
        {
            var janelas = await _janelaAtendimentoRepositorio.ListarAsync();
            return janelas.Select(janela => janela.ToDto()).ToList();
        }

        public async Task<JanelaAtendimentoDto?> BuscarPorIdAsync(int janelaId)
        {
            var janela = await _janelaAtendimentoRepositorio.ObterPorIdAsync(janelaId);
            return janela?.ToDto();
        }

        public Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return _janelaAtendimentoRepositorio.VeterinarioExisteAsync(veterinarioId);
        }

        public Task<bool> HorarioExisteAsync(int veterinarioId, DateTime dataHoraInicio, int? ignorarJanelaId = null)
        {
            return _janelaAtendimentoRepositorio.HorarioExisteAsync(veterinarioId, dataHoraInicio, ignorarJanelaId);
        }

        public async Task<bool> PossuiConsultaAsync(int janelaId)
        {
            var janela = await _janelaAtendimentoRepositorio.ObterParaVerificacaoAsync(janelaId);

            if (janela is null)
            {
                return false;
            }

            return await _consultaRepositorio.ExisteAtivaAsync(janela.VeterinarioId, janela.DataHoraInicio);
        }

        public async Task<JanelaAtendimentoDto> CadastrarAsync(SalvarJanelaAtendimentoRequest request)
        {
            var janela = new JanelaAtendimentoEntity
            {
                DataHoraInicio = request.DataHoraInicio!.Value,
                VeterinarioId = request.VeterinarioId
            };

            await _janelaAtendimentoRepositorio.AdicionarAsync(janela);
            await _unitOfWork.SalvarAsync();

            return (await BuscarPorIdAsync(janela.Id))!;
        }

        public async Task<JanelaAtendimentoDto?> AtualizarAsync(int janelaId, SalvarJanelaAtendimentoRequest request)
        {
            var janela = await _janelaAtendimentoRepositorio.ObterParaAlterarAsync(janelaId);

            if (janela is null)
            {
                return null;
            }

            janela.DataHoraInicio = request.DataHoraInicio!.Value;
            janela.VeterinarioId = request.VeterinarioId;

            await _unitOfWork.SalvarAsync();

            return await BuscarPorIdAsync(janela.Id);
        }

        public async Task<bool> RemoverAsync(int janelaId)
        {
            var janela = await _janelaAtendimentoRepositorio.ObterParaAlterarAsync(janelaId);

            if (janela is null)
            {
                return false;
            }

            _janelaAtendimentoRepositorio.Remover(janela);
            await _unitOfWork.SalvarAsync();

            return true;
        }
    }
}
