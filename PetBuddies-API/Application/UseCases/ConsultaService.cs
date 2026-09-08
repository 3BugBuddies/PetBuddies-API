using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Infrastructure.Clients;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Application.Dtos.Consulta;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.UseCases
{
    public class ConsultaService
    {
        private readonly ApplicationContext _context;
        private readonly MotorApiClient _motorApiClient;

        public ConsultaService(ApplicationContext context, MotorApiClient motorApiClient)
        {
            _context = context;
            _motorApiClient = motorApiClient;
        }

        public async Task<bool> AnimalExisteAsync(int animalId)
        {
            return await _context.Animais
                .AsNoTracking()
                .AnyAsync(animal => animal.Id == animalId);
        }

        public async Task<bool> JanelaExisteAsync(int janelaId)
        {
            return await _context.JanelasAtendimento
                .AsNoTracking()
                .AnyAsync(janela => janela.Id == janelaId);
        }

        public async Task<bool> JanelaOcupadaAsync(int janelaId, int? ignorarConsultaId = null)
        {
            var janela = await _context.JanelasAtendimento
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == janelaId);

            if (janela is null)
            {
                return false;
            }

            var dataHora = janela.DataHoraInicio;

            var query = _context.Consultas
                .AsNoTracking()
                .Where(consulta =>
                    consulta.VeterinarioId == janela.VeterinarioId
                    && consulta.DataHora == dataHora
                    && consulta.Status != StatusConsultaEnum.CANCELADA);

            if (ignorarConsultaId.HasValue)
                query = query.Where(consulta => consulta.Id != ignorarConsultaId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ConsultaRealizadaAsync(int consultaId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .AnyAsync(consulta => consulta.Id == consultaId && consulta.Status == StatusConsultaEnum.REALIZADA);
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

            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();

            return ToDto(consulta);
        }

        public async Task<List<ConsultaDto>> ListarAsync()
        {
            return await _context.Consultas
                .AsNoTracking()
                .OrderByDescending(consulta => consulta.DataHora)
                .Select(consulta => new ConsultaDto
                {
                    Id = consulta.Id,
                    TipoConsulta = consulta.TipoConsulta.ToString(),
                    DataHora = consulta.DataHora,
                    Status = consulta.Status.ToString(),
                    AnimalId = consulta.AnimalId
                })
                .ToListAsync();
        }

        public async Task<List<ConsultaDto>> ListarPorAnimalAsync(int animalId)
        {
            return await _context.Consultas
                .AsNoTracking()
                .Where(consulta => consulta.AnimalId == animalId)
                .OrderByDescending(consulta => consulta.DataHora)
                .Select(consulta => new ConsultaDto
                {
                    Id = consulta.Id,
                    TipoConsulta = consulta.TipoConsulta.ToString(),
                    DataHora = consulta.DataHora,
                    Status = consulta.Status.ToString(),
                    AnimalId = consulta.AnimalId
                })
                .ToListAsync();
        }

        public async Task<ConsultaDto?> BuscarPorIdAsync(int consultaId)
        {
            var consulta = await _context.Consultas
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == consultaId);

            return consulta is null ? null : ToDto(consulta);
        }

        public async Task<ConsultaDto?> CancelarAsync(int consultaId, AtualizarConsultaStatusRequest request)
        {
            var consulta = await _context.Consultas
                .SingleOrDefaultAsync(item => item.Id == consultaId);

            if (consulta is null)
            {
                return null;
            }

            consulta.Status = request.Status!.Value;
            consulta.Motivo = request.Motivo;

            await _context.SaveChangesAsync();

            return ToDto(consulta);
        }

        public async Task<ConsultaDto?> AtualizarAsync(int consultaId, AtualizarConsultaRequest request)
        {
            var consulta = await _context.Consultas
                .SingleOrDefaultAsync(item => item.Id == consultaId);

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

            await _context.SaveChangesAsync();

            return ToDto(consulta);
        }

        public async Task<bool> RemoverAsync(int consultaId)
        {
            var consulta = await _context.Consultas
                .SingleOrDefaultAsync(item => item.Id == consultaId);

            if (consulta is null)
            {
                return false;
            }

            _context.Consultas.Remove(consulta);
            await _context.SaveChangesAsync();

            return true;
        }

        private Task<JanelaAtendimentoEntity?> BuscarJanelaAsync(int janelaId)
        {
            return _context.JanelasAtendimento
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == janelaId);
        }

        private static ConsultaDto ToDto(ConsultaEntity consulta)
        {
            return new ConsultaDto
            {
                Id = consulta.Id,
                TipoConsulta = consulta.TipoConsulta.ToString(),
                DataHora = consulta.DataHora,
                Status = consulta.Status.ToString(),
                AnimalId = consulta.AnimalId
            };
        }
    }
}
