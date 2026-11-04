using PetBuddies_API.Domain.Documents;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.UseCases
{
    // Regra pura, sem banco: recebe os fatos do período e as regras vigentes da clínica
    // e devolve uma parcela por critério. Critério sem regra vigente vale zero.
    public static class CalculadoraDePontos
    {
        public const decimal AdesaoMinimaParaBonus = 0.9m;

        public static List<ParcelaCalculo> Calcular(EntradaCalculo entrada, IReadOnlyCollection<RegraPontuacaoEntity> regrasVigentes)
        {
            var adesao = entrada.DosesPrevistas == 0
                ? 0m
                : (decimal)entrada.DosesComCheckin / entrada.DosesPrevistas;
            var atingiuAdesao = entrada.DosesPrevistas > 0 && adesao >= AdesaoMinimaParaBonus;

            return new List<ParcelaCalculo>
            {
                Parcela(regrasVigentes, TipoGestoEnum.CONSULTA_REALIZADA, 1,
                    "Consulta realizada."),
                Parcela(regrasVigentes, TipoGestoEnum.CHECKIN_REGISTRADO, entrada.DosesComCheckin,
                    $"{entrada.DosesComCheckin} de {entrada.DosesPrevistas} doses com check-in no período."),
                Parcela(regrasVigentes, TipoGestoEnum.ADESAO_COMPLETA, atingiuAdesao ? 1 : 0,
                    entrada.DosesPrevistas == 0
                        ? "Sem doses previstas no período."
                        : $"Adesão de {Math.Round(adesao * 100)}%; mínimo para o bônus: {AdesaoMinimaParaBonus * 100:0}%."),
                Parcela(regrasVigentes, TipoGestoEnum.EVOLUCAO_POSITIVA, entrada.Evolucao == EvolucaoClinicaEnum.MELHOROU ? 1 : 0,
                    entrada.Evolucao is null
                        ? "Evolução não informada pelo veterinário."
                        : $"Evolução informada: {entrada.Evolucao}.")
            };
        }

        private static ParcelaCalculo Parcela(
            IReadOnlyCollection<RegraPontuacaoEntity> regrasVigentes,
            TipoGestoEnum gesto,
            int quantidade,
            string observacao)
        {
            var regra = regrasVigentes.FirstOrDefault(candidata => candidata.Gesto == gesto);

            if (regra is null)
            {
                return new ParcelaCalculo
                {
                    Gesto = gesto,
                    Quantidade = quantidade,
                    Observacao = $"{observacao} Sem regra vigente para a clínica na data da consulta."
                };
            }

            return new ParcelaCalculo
            {
                Gesto = gesto,
                Quantidade = quantidade,
                PontosPorUnidade = regra.Pontos,
                Pontos = regra.Pontos * quantidade,
                RegraId = regra.Id,
                InicioVigencia = regra.InicioVigencia.ToDateTime(TimeOnly.MinValue),
                Observacao = observacao
            };
        }
    }
}
