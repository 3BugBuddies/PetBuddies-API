using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.JanelaAtendimento;
using PetBuddies_API.Enums;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class JanelaAtendimentoService
    {
        private readonly ApplicationContext _context;

        public JanelaAtendimentoService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<JanelaAtendimentoDto>> ListarDisponiveisAsync()
        {
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            var inicioHoje = hoje.ToDateTime(TimeOnly.MinValue);

            var consultasOcupadas = await _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.Status != StatusConsultaEnum.CANCELADA && consulta.DataHora >= inicioHoje)
                .Select(consulta => new { consulta.VeterinarioId, consulta.DataHora })
                .ToListAsync();

            var ocupadas = consultasOcupadas
                .Select(consulta => (consulta.VeterinarioId, consulta.DataHora))
                .ToHashSet();

            var janelas = await _context.JanelasAtendimento
                .AsNoTracking()
                .Include(janela => janela.Veterinario)
                .Where(janela => janela.Data >= hoje)
                .OrderBy(janela => janela.Data)
                .ThenBy(janela => janela.HoraInicio)
                .ToListAsync();

            return janelas
                .Where(janela => !ocupadas.Contains((janela.VeterinarioId, janela.Data.ToDateTime(janela.HoraInicio))))
                .Select(ToDto)
                .ToList();
        }

        public async Task<List<JanelaAtendimentoDto>> ListarAsync()
        {
            var janelas = await _context.JanelasAtendimento
                .AsNoTracking()
                .Include(janela => janela.Veterinario)
                .OrderBy(janela => janela.Data)
                .ThenBy(janela => janela.HoraInicio)
                .ToListAsync();

            return janelas.Select(ToDto).ToList();
        }

        public async Task<JanelaAtendimentoDto?> BuscarPorIdAsync(int janelaId)
        {
            var janela = await _context.JanelasAtendimento
                .AsNoTracking()
                .Include(item => item.Veterinario)
                .SingleOrDefaultAsync(item => item.Id == janelaId);

            return janela is null ? null : ToDto(janela);
        }

        public Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return _context.Veterinarios
                .AsNoTracking()
                .AnyAsync(item => item.Id == veterinarioId);
        }

        public Task<bool> HorarioExisteAsync(int veterinarioId, DateOnly data, TimeOnly horaInicio, int? ignorarJanelaId = null)
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .AnyAsync(item =>
                    item.VeterinarioId == veterinarioId
                    && item.Data == data
                    && item.HoraInicio == horaInicio
                    && (!ignorarJanelaId.HasValue || item.Id != ignorarJanelaId.Value));
        }

        public async Task<bool> PossuiConsultaAsync(int janelaId)
        {
            var janela = await _context.JanelasAtendimento
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == janelaId);

            if (janela is null)
            {
                return false;
            }

            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(consulta =>
                    consulta.VeterinarioId == janela.VeterinarioId
                    && consulta.DataHora == janela.Data.ToDateTime(janela.HoraInicio)
                    && consulta.Status != StatusConsultaEnum.CANCELADA);
        }

        public async Task<JanelaAtendimentoDto> CadastrarAsync(SalvarJanelaAtendimentoRequest request)
        {
            var janela = new JanelaAtendimentoEntity
            {
                Data = request.Data!.Value,
                HoraInicio = request.HoraInicio!.Value,
                HoraFim = request.HoraFim!.Value,
                DuracaoSlot = request.DuracaoSlot,
                VeterinarioId = request.VeterinarioId
            };

            _context.JanelasAtendimento.Add(janela);
            await _context.SaveChangesAsync();

            return (await BuscarPorIdAsync(janela.Id))!;
        }

        public async Task<JanelaAtendimentoDto?> AtualizarAsync(int janelaId, SalvarJanelaAtendimentoRequest request)
        {
            var janela = await _context.JanelasAtendimento
                .SingleOrDefaultAsync(item => item.Id == janelaId);

            if (janela is null)
            {
                return null;
            }

            janela.Data = request.Data!.Value;
            janela.HoraInicio = request.HoraInicio!.Value;
            janela.HoraFim = request.HoraFim!.Value;
            janela.DuracaoSlot = request.DuracaoSlot;
            janela.VeterinarioId = request.VeterinarioId;

            await _context.SaveChangesAsync();

            return await BuscarPorIdAsync(janela.Id);
        }

        public async Task<bool> RemoverAsync(int janelaId)
        {
            var janela = await _context.JanelasAtendimento
                .SingleOrDefaultAsync(item => item.Id == janelaId);

            if (janela is null)
            {
                return false;
            }

            _context.JanelasAtendimento.Remove(janela);
            await _context.SaveChangesAsync();

            return true;
        }

        private static JanelaAtendimentoDto ToDto(JanelaAtendimentoEntity janela)
        {
            return new JanelaAtendimentoDto
            {
                Id = janela.Id,
                Data = janela.Data,
                HoraInicio = janela.HoraInicio,
                HoraFim = janela.HoraFim,
                VeterinarioId = janela.VeterinarioId,
                VeterinarioNome = janela.Veterinario?.Nome ?? string.Empty
            };
        }
    }
}
