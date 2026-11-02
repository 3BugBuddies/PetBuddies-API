using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Documents;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Tests.Unit.App
{
    public class CalculadoraDePontosTest
    {
        private static RegraPontuacaoEntity Regra(TipoGestoEnum gesto, int pontos) => new()
        {
            Id = (int)gesto + 1,
            ClinicaId = 1,
            Gesto = gesto,
            Pontos = pontos,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        private static readonly List<RegraPontuacaoEntity> TodasAsRegras = new()
        {
            Regra(TipoGestoEnum.CONSULTA_REALIZADA, 10),
            Regra(TipoGestoEnum.CHECKIN_REGISTRADO, 1),
            Regra(TipoGestoEnum.ADESAO_COMPLETA, 15),
            Regra(TipoGestoEnum.EVOLUCAO_POSITIVA, 5)
        };

        private static EntradaCalculo Entrada(int previstas, int comCheckin, EvolucaoClinicaEnum? evolucao) => new()
        {
            DosesPrevistas = previstas,
            DosesComCheckin = comCheckin,
            DiasNoPeriodo = 30,
            DiasComCheckin = comCheckin,
            Evolucao = evolucao
        };

        private static int PontosDo(List<ParcelaCalculo> parcelas, TipoGestoEnum gesto)
            => parcelas.Single(parcela => parcela.Gesto == gesto).Pontos;

        [Fact]
        [Trait("Calculadora", "Pontuacao")]
        public void Calcular_TodosOsCriteriosAtingidos_SomaAsQuatroParcelas()
        {
            // Arrange — 18 de 20 doses = 90%, no limite do bônus.
            var entrada = Entrada(20, 18, EvolucaoClinicaEnum.MELHOROU);

            // Act
            var parcelas = CalculadoraDePontos.Calcular(entrada, TodasAsRegras);

            // Assert — 10 + 18×1 + 15 + 5
            Assert.Equal(48, parcelas.Sum(parcela => parcela.Pontos));
        }

        [Fact]
        [Trait("Calculadora", "Pontuacao")]
        public void Calcular_AdesaoAbaixoDoMinimo_NaoDaBonusDeAdesao()
        {
            // Arrange — 17 de 20 = 85%.
            var entrada = Entrada(20, 17, null);

            // Act
            var parcelas = CalculadoraDePontos.Calcular(entrada, TodasAsRegras);

            // Assert
            Assert.Equal(0, PontosDo(parcelas, TipoGestoEnum.ADESAO_COMPLETA));
            Assert.Equal(17, PontosDo(parcelas, TipoGestoEnum.CHECKIN_REGISTRADO));
        }

        [Fact]
        [Trait("Calculadora", "Pontuacao")]
        public void Calcular_SemDosesPrevistas_NaoDaBonusDeAdesao()
        {
            // Arrange — check-up sem prescrição no período.
            var entrada = Entrada(0, 0, null);

            // Act
            var parcelas = CalculadoraDePontos.Calcular(entrada, TodasAsRegras);

            // Assert
            Assert.Equal(0, PontosDo(parcelas, TipoGestoEnum.ADESAO_COMPLETA));
            Assert.Equal(10, parcelas.Sum(parcela => parcela.Pontos));
        }

        [Theory]
        [InlineData(EvolucaoClinicaEnum.ESTAVEL)]
        [InlineData(EvolucaoClinicaEnum.PIOROU)]
        [Trait("Calculadora", "Pontuacao")]
        public void Calcular_EvolucaoNaoPositiva_NaoDaBonusNemTiraPontos(EvolucaoClinicaEnum evolucao)
        {
            // Arrange
            var entrada = Entrada(10, 10, evolucao);

            // Act
            var parcelas = CalculadoraDePontos.Calcular(entrada, TodasAsRegras);

            // Assert
            Assert.Equal(0, PontosDo(parcelas, TipoGestoEnum.EVOLUCAO_POSITIVA));
            Assert.All(parcelas, parcela => Assert.True(parcela.Pontos >= 0));
        }

        [Fact]
        [Trait("Calculadora", "Pontuacao")]
        public void Calcular_SemRegraVigente_ParcelaValeZeroComObservacao()
        {
            // Arrange — clínica só configurou a consulta realizada.
            var regras = new List<RegraPontuacaoEntity> { Regra(TipoGestoEnum.CONSULTA_REALIZADA, 10) };
            var entrada = Entrada(20, 20, EvolucaoClinicaEnum.MELHOROU);

            // Act
            var parcelas = CalculadoraDePontos.Calcular(entrada, regras);

            // Assert
            var checkin = parcelas.Single(parcela => parcela.Gesto == TipoGestoEnum.CHECKIN_REGISTRADO);
            Assert.Equal(0, checkin.Pontos);
            Assert.Null(checkin.RegraId);
            Assert.Contains("Sem regra vigente", checkin.Observacao);
            Assert.Equal(10, parcelas.Sum(parcela => parcela.Pontos));
        }

        [Fact]
        [Trait("Calculadora", "Pontuacao")]
        public void Calcular_ComRegra_ParcelaGuardaARegraUsada()
        {
            // Arrange
            var entrada = Entrada(20, 18, null);

            // Act
            var parcelas = CalculadoraDePontos.Calcular(entrada, TodasAsRegras);

            // Assert
            var consulta = parcelas.Single(parcela => parcela.Gesto == TipoGestoEnum.CONSULTA_REALIZADA);
            Assert.Equal(TodasAsRegras[0].Id, consulta.RegraId);
            Assert.Equal(new DateTime(2026, 1, 1), consulta.InicioVigencia);
            Assert.Equal(10, consulta.PontosPorUnidade);
        }
    }
}
