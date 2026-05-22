using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.Procedimento;
using PetBuddies_API.Enums;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class ProcedimentoService
    {
        private readonly ApplicationContext _context;
        private readonly MotorApiClient _motorApiClient;

        public ProcedimentoService(ApplicationContext context, MotorApiClient motorApiClient)
        {
            _context = context;
            _motorApiClient = motorApiClient;
        }

        public async Task<List<ProcedimentoDto>> ListarAsync(int? animalId = null)
        {
            var query = _context.Procedimentos.AsNoTracking();

            if (animalId.HasValue)
            {
                query = query.Where(procedimento => procedimento.AnimalId == animalId.Value);
            }

            var procedimentos = await query
                .OrderByDescending(procedimento => procedimento.DataPrevistaInicio)
                .ToListAsync();

            return procedimentos.Select(ToDto).ToList();
        }

        public async Task<ProcedimentoDto?> BuscarPorIdAsync(int procedimentoId)
        {
            var procedimento = await _context.Procedimentos
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == procedimentoId);

            return procedimento is null ? null : ToDto(procedimento);
        }

        public async Task<bool> AnimalExisteAsync(int animalId)
        {
            return await _context.Animais
                .AsNoTracking()
                .AnyAsync(item => item.Id == animalId);
        }

        public async Task<bool> VeterinarioExisteAsync(int veterinarioId)
        {
            return await _context.Veterinarios
                .AsNoTracking()
                .AnyAsync(item => item.Id == veterinarioId);
        }

        public async Task<bool> RegistroAtendimentoPertenceAoAnimalAsync(int registroAtendimentoId, int animalId)
        {
            return await _context.RegistrosAtendimento
                .AsNoTracking()
                .AnyAsync(item => item.Id == registroAtendimentoId && item.AnimalId == animalId);
        }

        public async Task<ProcedimentoDto> CadastrarAsync(SalvarProcedimentoRequest request)
        {
            var procedimento = new ProcedimentoEntity();
            Aplicar(procedimento, request);

            _context.Procedimentos.Add(procedimento);
            await _context.SaveChangesAsync();

            await DispararPlanoPosCirurgicoSeNecessarioAsync(procedimento);

            return ToDto(procedimento);
        }

        public async Task<ProcedimentoDto?> AtualizarAsync(int procedimentoId, SalvarProcedimentoRequest request)
        {
            var procedimento = await _context.Procedimentos.SingleOrDefaultAsync(item => item.Id == procedimentoId);

            if (procedimento is null)
            {
                return null;
            }

            var jaEraCirurgiaRealizada = EhCirurgiaRealizada(procedimento);
            Aplicar(procedimento, request);

            await _context.SaveChangesAsync();

            if (!jaEraCirurgiaRealizada)
            {
                await DispararPlanoPosCirurgicoSeNecessarioAsync(procedimento);
            }

            return ToDto(procedimento);
        }

        public async Task<bool> RemoverAsync(int procedimentoId)
        {
            var procedimento = await _context.Procedimentos.SingleOrDefaultAsync(item => item.Id == procedimentoId);

            if (procedimento is null)
            {
                return false;
            }

            _context.Procedimentos.Remove(procedimento);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task DispararPlanoPosCirurgicoSeNecessarioAsync(ProcedimentoEntity procedimento)
        {
            if (!EhCirurgiaRealizada(procedimento))
            {
                return;
            }

            var consultaId = await _context.RegistrosAtendimento
                .AsNoTracking()
                .Where(registro => registro.Id == procedimento.RegistroAtendimentoId)
                .Select(registro => registro.ConsultaId)
                .SingleAsync();

            await _motorApiClient.InstanciarPlanoPosCirurgicoAsync(procedimento.AnimalId, consultaId);
        }

        private static bool EhCirurgiaRealizada(ProcedimentoEntity procedimento)
        {
            return procedimento.Tipo == TipoProcedimentoEnum.CIRURGIA
                && procedimento.Status == StatusProcedimentoEnum.REALIZADO;
        }

        private static void Aplicar(ProcedimentoEntity procedimento, SalvarProcedimentoRequest request)
        {
            var dataInicio = request.DataPrevistaInicio ?? DateTime.Now;

            procedimento.Tipo = request.Tipo!.Value;
            procedimento.Nome = request.Nome.Trim();
            procedimento.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
            procedimento.Status = request.Status!.Value;
            procedimento.DataPrevistaInicio = dataInicio;
            procedimento.DataPrevistaFim = request.DataPrevistaFim ?? dataInicio;
            procedimento.AnexosUrl = string.IsNullOrWhiteSpace(request.AnexosUrl) ? null : request.AnexosUrl.Trim();
            procedimento.Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim();
            procedimento.RegistroAtendimentoId = request.RegistroAtendimentoId;
            procedimento.AnimalId = request.AnimalId;
            procedimento.VeterinarioId = request.VeterinarioId;
        }

        private static ProcedimentoDto ToDto(ProcedimentoEntity procedimento)
        {
            return new ProcedimentoDto
            {
                Id = procedimento.Id,
                Tipo = procedimento.Tipo,
                Nome = procedimento.Nome,
                Descricao = procedimento.Descricao,
                Status = procedimento.Status,
                DataPrevistaInicio = procedimento.DataPrevistaInicio,
                DataPrevistaFim = procedimento.DataPrevistaFim,
                AnexosUrl = procedimento.AnexosUrl,
                Observacao = procedimento.Observacao,
                RegistroAtendimentoId = procedimento.RegistroAtendimentoId,
                AnimalId = procedimento.AnimalId,
                VeterinarioId = procedimento.VeterinarioId
            };
        }
    }
}
