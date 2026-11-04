using PetBuddies_API.Application.Dtos.Pontuacao;
using PetBuddies_API.Domain.Documents;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class PontuacaoMapper
    {
        public static LancamentoPontosDto ToDto(this LancamentoPontosEntity lancamento)
        {
            return new LancamentoPontosDto
            {
                Id = lancamento.Id,
                ConsultaId = lancamento.ConsultaId,
                ResponsavelId = lancamento.ResponsavelId,
                AnimalId = lancamento.AnimalId,
                ClinicaId = lancamento.ClinicaId,
                DataConsulta = lancamento.DataConsulta,
                Pontos = lancamento.Pontos,
                CreatedAt = lancamento.CreatedAt
            };
        }

        public static MemoriaCalculoDto ToDto(this MemoriaCalculoDocument memoria)
        {
            return new MemoriaCalculoDto
            {
                ConsultaId = memoria.ConsultaId,
                LancamentoId = memoria.LancamentoId,
                ResponsavelId = memoria.ResponsavelId,
                AnimalId = memoria.AnimalId,
                ClinicaId = memoria.ClinicaId,
                PeriodoDe = memoria.Periodo.De,
                PeriodoAte = memoria.Periodo.Ate,
                DosesPrevistas = memoria.Entrada.DosesPrevistas,
                DosesComCheckin = memoria.Entrada.DosesComCheckin,
                DiasNoPeriodo = memoria.Entrada.DiasNoPeriodo,
                DiasComCheckin = memoria.Entrada.DiasComCheckin,
                AcionamentosClinica = memoria.Entrada.AcionamentosClinica,
                Evolucao = memoria.Entrada.Evolucao,
                Parcelas = memoria.Parcelas.Select(parcela => new ParcelaCalculoDto
                {
                    Gesto = parcela.Gesto,
                    Quantidade = parcela.Quantidade,
                    PontosPorUnidade = parcela.PontosPorUnidade,
                    Pontos = parcela.Pontos,
                    RegraId = parcela.RegraId,
                    InicioVigencia = parcela.InicioVigencia is null
                        ? null
                        : DateOnly.FromDateTime(parcela.InicioVigencia.Value),
                    Observacao = parcela.Observacao
                }).ToList(),
                Total = memoria.Total,
                CalculadoEm = memoria.CalculadoEm
            };
        }

        public static EntradaCalculo ToEntrada(this PontuarConsultaRequest request)
        {
            return new EntradaCalculo
            {
                DosesPrevistas = request.DosesPrevistas!.Value,
                DosesComCheckin = request.DosesComCheckin!.Value,
                DiasNoPeriodo = request.DiasNoPeriodo!.Value,
                DiasComCheckin = request.DiasComCheckin!.Value,
                AcionamentosClinica = request.AcionamentosClinica!.Value,
                Evolucao = request.Evolucao
            };
        }
    }
}
