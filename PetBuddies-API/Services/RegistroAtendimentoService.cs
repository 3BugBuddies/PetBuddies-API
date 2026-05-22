using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Data;
using PetBuddies_API.Dtos.RegistroAtendimento;
using PetBuddies_API.Models;

namespace PetBuddies_API.Services
{
    public class RegistroAtendimentoService
    {
        private readonly ApplicationContext _context;

        public RegistroAtendimentoService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<RegistroAtendimentoDto>> ListarAsync(int? animalId = null)
        {
            var query = _context.RegistrosAtendimento.AsNoTracking();

            if (animalId.HasValue)
            {
                query = query.Where(registro => registro.AnimalId == animalId.Value);
            }

            var registros = await query
                .OrderByDescending(registro => registro.DataAtendimento)
                .ToListAsync();

            return registros.Select(ToDto).ToList();
        }

        public async Task<RegistroAtendimentoDto?> BuscarPorIdAsync(int registroAtendimentoId)
        {
            var registro = await _context.RegistrosAtendimento
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == registroAtendimentoId);

            return registro is null ? null : ToDto(registro);
        }

        public async Task<bool> AnimalExisteAsync(int animalId)
        {
            return await _context.Animais
                .AsNoTracking()
                .AnyAsync(item => item.Id == animalId);
        }

        public async Task<bool> ConsultaPertenceAoAnimalAsync(int consultaId, int animalId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(item => item.Id == consultaId && item.AnimalId == animalId);
        }

        public async Task<bool> ProntuarioPertenceAoAnimalAsync(int prontuarioId, int animalId)
        {
            return await _context.Prontuarios
                .AsNoTracking()
                .AnyAsync(item => item.Id == prontuarioId && item.AnimalId == animalId);
        }

        public async Task<RegistroAtendimentoDto> CadastrarAsync(SalvarRegistroAtendimentoRequest request)
        {
            var prontuarioId = await ResolverProntuarioIdAsync(request.AnimalId, request.ProntuarioId);
            var registro = new RegistroAtendimentoEntity();
            Aplicar(registro, request, prontuarioId);

            _context.RegistrosAtendimento.Add(registro);
            await _context.SaveChangesAsync();

            return ToDto(registro);
        }

        public async Task<RegistroAtendimentoDto?> AtualizarAsync(int registroAtendimentoId, SalvarRegistroAtendimentoRequest request)
        {
            var registro = await _context.RegistrosAtendimento.SingleOrDefaultAsync(item => item.Id == registroAtendimentoId);

            if (registro is null)
            {
                return null;
            }

            var prontuarioId = await ResolverProntuarioIdAsync(request.AnimalId, request.ProntuarioId);
            Aplicar(registro, request, prontuarioId);

            await _context.SaveChangesAsync();

            return ToDto(registro);
        }

        public async Task<bool> RemoverAsync(int registroAtendimentoId)
        {
            var registro = await _context.RegistrosAtendimento.SingleOrDefaultAsync(item => item.Id == registroAtendimentoId);

            if (registro is null)
            {
                return false;
            }

            _context.RegistrosAtendimento.Remove(registro);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<int> ResolverProntuarioIdAsync(int animalId, int? prontuarioId)
        {
            if (prontuarioId.HasValue)
            {
                return prontuarioId.Value;
            }

            var prontuarioExistente = await _context.Prontuarios
                .AsNoTracking()
                .OrderBy(prontuario => prontuario.Id)
                .FirstOrDefaultAsync(prontuario => prontuario.AnimalId == animalId);

            if (prontuarioExistente is not null)
            {
                return prontuarioExistente.Id;
            }

            var prontuario = new ProntuarioEntity
            {
                AnimalId = animalId,
                Alergias = null,
                Observacoes = null
            };

            _context.Prontuarios.Add(prontuario);
            await _context.SaveChangesAsync();

            return prontuario.Id;
        }

        private static void Aplicar(RegistroAtendimentoEntity registro, SalvarRegistroAtendimentoRequest request, int prontuarioId)
        {
            registro.DataAtendimento = request.DataAtendimento ?? DateTime.Now;
            registro.Anamnese = string.IsNullOrWhiteSpace(request.Anamnese) ? null : request.Anamnese.Trim();
            registro.Diagnostico = string.IsNullOrWhiteSpace(request.Diagnostico) ? null : request.Diagnostico.Trim();
            registro.Tratamento = string.IsNullOrWhiteSpace(request.Tratamento) ? null : request.Tratamento.Trim();
            registro.Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim();
            registro.AnimalId = request.AnimalId;
            registro.ProntuarioId = prontuarioId;
            registro.ConsultaId = request.ConsultaId;
        }

        private static RegistroAtendimentoDto ToDto(RegistroAtendimentoEntity registro)
        {
            return new RegistroAtendimentoDto
            {
                Id = registro.Id,
                DataAtendimento = registro.DataAtendimento,
                Anamnese = registro.Anamnese,
                Diagnostico = registro.Diagnostico,
                Tratamento = registro.Tratamento,
                Observacao = registro.Observacao,
                AnimalId = registro.AnimalId,
                ProntuarioId = registro.ProntuarioId,
                ConsultaId = registro.ConsultaId
            };
        }
    }
}
