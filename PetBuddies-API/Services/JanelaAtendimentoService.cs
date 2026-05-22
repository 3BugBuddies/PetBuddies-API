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
            var agora = DateTime.Now;

            var consultasOcupadas = await _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.Status != StatusConsultaEnum.CANCELADA && consulta.DataHora >= agora)
                .Select(consulta => new { consulta.VeterinarioId, consulta.DataHora })
                .ToListAsync();

            var ocupadas = consultasOcupadas
                .Select(consulta => (consulta.VeterinarioId, consulta.DataHora))
                .ToHashSet();

            var janelas = await _context.JanelasAtendimento
                .AsNoTracking()
                .Include(janela => janela.Veterinario)
                .Where(janela => janela.DataHoraInicio >= agora)
                .OrderBy(janela => janela.DataHoraInicio)
                .ToListAsync();

            return janelas
                .Where(janela => !ocupadas.Contains((janela.VeterinarioId, janela.DataHoraInicio)))
                .Select(ToDto)
                .ToList();
        }

        public async Task<List<JanelaAtendimentoDto>> ListarAsync()
        {
            var janelas = await _context.JanelasAtendimento
                .AsNoTracking()
                .Include(janela => janela.Veterinario)
                .OrderBy(janela => janela.DataHoraInicio)
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

        public async Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return await _context.Veterinarios
                .AsNoTracking()
                .AnyAsync(item => item.Id == veterinarioId);
        }

        public async Task<bool> HorarioExisteAsync(int veterinarioId, DateTime dataHoraInicio, int? ignorarJanelaId = null)
        {
            var query = _context.JanelasAtendimento
                .AsNoTracking()
                .Where(item =>
                    item.VeterinarioId == veterinarioId
                    && item.DataHoraInicio == dataHoraInicio);

            if (ignorarJanelaId.HasValue)
                query = query.Where(item => item.Id != ignorarJanelaId.Value);

            return await query.AnyAsync();
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
                    && consulta.DataHora == janela.DataHoraInicio
                    && consulta.Status != StatusConsultaEnum.CANCELADA);
        }

        public async Task<JanelaAtendimentoDto> CadastrarAsync(SalvarJanelaAtendimentoRequest request)
        {
            var janela = new JanelaAtendimentoEntity
            {
                DataHoraInicio = request.DataHoraInicio!.Value,
                DataHoraFim = request.DataHoraFim!.Value,
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

            janela.DataHoraInicio = request.DataHoraInicio!.Value;
            janela.DataHoraFim = request.DataHoraFim!.Value;
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
                DataHoraInicio = janela.DataHoraInicio,
                DataHoraFim = janela.DataHoraFim,
                DuracaoSlot = janela.DuracaoSlot,
                VeterinarioId = janela.VeterinarioId,
                VeterinarioNome = janela.Veterinario?.Nome ?? string.Empty
            };
        }
    }
}
