using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Application.Dtos.Pontuacao;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Application.Mappers;
using PetBuddies_API.Domain.Documents;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Application.UseCases
{
    public class PontuacaoService : IPontuacaoService
    {
        private readonly ILancamentoPontosRepository _lancamentos;
        private readonly IRegraPontuacaoRepository _regras;
        private readonly IMemoriaCalculoRepository _memorias;
        private readonly ILogger<PontuacaoService> _logger;

        public PontuacaoService(
            ILancamentoPontosRepository lancamentos,
            IRegraPontuacaoRepository regras,
            IMemoriaCalculoRepository memorias,
            ILogger<PontuacaoService> logger)
        {
            _lancamentos = lancamentos;
            _regras = regras;
            _memorias = memorias;
            _logger = logger;
        }

        public string? Validar(PontuarConsultaRequest request)
        {
            if (request.ConsultaId is null or <= 0
                || request.ResponsavelId is null or <= 0
                || request.AnimalId is null or <= 0)
            {
                return "ConsultaId, ResponsavelId e AnimalId são obrigatórios e maiores que zero.";
            }

            if (request.ClinicaId <= 0)
            {
                return "ClinicaId deve ser maior que zero.";
            }

            if (request.DataConsulta is null)
            {
                return "Data da consulta é obrigatória.";
            }

            if (request.DataConsultaAnterior is not null && request.DataConsultaAnterior >= request.DataConsulta)
            {
                return "A consulta anterior tem de ser antes da consulta pontuada.";
            }

            if (request.DosesPrevistas is null or < 0
                || request.DosesComCheckin is null or < 0
                || request.DiasNoPeriodo is null or < 0
                || request.DiasComCheckin is null or < 0
                || request.AcionamentosClinica is null or < 0)
            {
                return "As contagens do período são obrigatórias e não podem ser negativas.";
            }

            if (request.DosesComCheckin > request.DosesPrevistas)
            {
                return "Doses com check-in não podem passar das doses previstas.";
            }

            if (request.DiasComCheckin > request.DiasNoPeriodo)
            {
                return "Dias com check-in não podem passar dos dias no período.";
            }

            if (request.Evolucao is not null && !Enum.IsDefined(request.Evolucao.Value))
            {
                return "Evolução clínica inválida.";
            }

            return null;
        }

        public async Task<ResultadoPontuacaoDto> PontuarConsultaAsync(
            PontuarConsultaRequest request,
            CancellationToken cancellationToken = default)
        {
            var consultaId = request.ConsultaId!.Value;

            // A mesma consulta não pontua duas vezes: o reenvio do Java devolve o que já existe.
            var existente = await _lancamentos.ObterPorConsultaAsync(consultaId, cancellationToken);
            if (existente is not null)
            {
                return new ResultadoPontuacaoDto(existente.ToDto(), JaExistia: true);
            }

            var dataConsulta = request.DataConsulta!.Value;
            var regrasVigentes = await _regras.ListarVigentesAsync(
                request.ClinicaId,
                DateOnly.FromDateTime(dataConsulta),
                cancellationToken);

            var entrada = request.ToEntrada();
            var parcelas = CalculadoraDePontos.Calcular(entrada, regrasVigentes);

            var lancamento = new LancamentoPontosEntity
            {
                ConsultaId = consultaId,
                ResponsavelId = request.ResponsavelId!.Value,
                AnimalId = request.AnimalId!.Value,
                ClinicaId = request.ClinicaId,
                DataConsulta = dataConsulta,
                Pontos = parcelas.Sum(parcela => parcela.Pontos)
            };

            try
            {
                await _lancamentos.AdicionarAsync(lancamento, cancellationToken);
            }
            catch (DbUpdateException)
            {
                // Dois envios simultâneos da mesma consulta: UK_LANCAMENTO_CONSULTA barra o
                // segundo, e ele devolve o lançamento que o primeiro gravou.
                var gravado = await _lancamentos.ObterPorConsultaAsync(consultaId, cancellationToken);
                if (gravado is null)
                {
                    throw;
                }

                return new ResultadoPontuacaoDto(gravado.ToDto(), JaExistia: true);
            }

            await SalvarMemoriaAsync(request, lancamento, entrada, parcelas, cancellationToken);

            return new ResultadoPontuacaoDto(lancamento.ToDto(), JaExistia: false);
        }

        public async Task<SaldoPontosDto> ObterSaldoAsync(long responsavelId, CancellationToken cancellationToken = default)
        {
            var saldo = await _lancamentos.SomarPontosAsync(responsavelId, cancellationToken);
            return new SaldoPontosDto { ResponsavelId = responsavelId, Saldo = saldo };
        }

        public async Task<List<LancamentoPontosDto>> ListarExtratoAsync(long responsavelId, CancellationToken cancellationToken = default)
        {
            var lancamentos = await _lancamentos.ListarPorResponsavelAsync(responsavelId, cancellationToken);
            return lancamentos.Select(lancamento => lancamento.ToDto()).ToList();
        }

        public async Task<MemoriaCalculoDto?> ObterMemoriaAsync(long consultaId, CancellationToken cancellationToken = default)
        {
            var memoria = await _memorias.ObterPorConsultaAsync(consultaId, cancellationToken);
            return memoria?.ToDto();
        }

        // O lançamento no Oracle é o valor oficial e já está gravado: falha no MongoDB vira
        // log, não erro para o Java, que reenviaria uma consulta já pontuada.
        private async Task SalvarMemoriaAsync(
            PontuarConsultaRequest request,
            LancamentoPontosEntity lancamento,
            EntradaCalculo entrada,
            List<ParcelaCalculo> parcelas,
            CancellationToken cancellationToken)
        {
            var memoria = new MemoriaCalculoDocument
            {
                ConsultaId = lancamento.ConsultaId,
                LancamentoId = lancamento.Id,
                ResponsavelId = lancamento.ResponsavelId,
                AnimalId = lancamento.AnimalId,
                ClinicaId = lancamento.ClinicaId,
                Periodo = new PeriodoCalculo { De = request.DataConsultaAnterior, Ate = lancamento.DataConsulta },
                Entrada = entrada,
                Parcelas = parcelas,
                Total = lancamento.Pontos,
                CalculadoEm = DateTime.Now
            };

            try
            {
                await _memorias.SalvarAsync(memoria, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Memória de cálculo da consulta {ConsultaId} não gravada no MongoDB; lançamento {LancamentoId} mantido",
                    lancamento.ConsultaId, lancamento.Id);
            }
        }
    }
}
