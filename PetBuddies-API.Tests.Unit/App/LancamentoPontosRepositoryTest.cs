using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Repositories;

namespace PetBuddies_API.Tests.Unit.App
{
    public class LancamentoPontosRepositoryTest
    {
        private readonly ApplicationContext _contexto;
        private readonly LancamentoPontosRepository _repositorio;

        public LancamentoPontosRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new ApplicationContext(options);
            _repositorio = new LancamentoPontosRepository(_contexto);
        }

        private static LancamentoPontosEntity Lancamento(long consultaId, long responsavelId, int pontos, DateTime dataConsulta) => new()
        {
            ConsultaId = consultaId,
            ResponsavelId = responsavelId,
            AnimalId = 3,
            ClinicaId = 1,
            DataConsulta = dataConsulta,
            Pontos = pontos
        };

        [Fact]
        [Trait("Repository", "LancamentoPontos")]
        public async Task SomarPontosAsync_VariosLancamentos_SomaSoOsDoResponsavel()
        {
            // Arrange
            _contexto.LancamentosPontos.AddRange(
                Lancamento(1, 1, 30, new DateTime(2026, 9, 1)),
                Lancamento(2, 1, 12, new DateTime(2026, 10, 1)),
                Lancamento(3, 2, 99, new DateTime(2026, 10, 1)));
            await _contexto.SaveChangesAsync();

            // Act
            var saldo = await _repositorio.SomarPontosAsync(1);

            // Assert
            Assert.Equal(42, saldo);
        }

        [Fact]
        [Trait("Repository", "LancamentoPontos")]
        public async Task SomarPontosAsync_SemLancamentos_RetornaZero()
        {
            // Act
            var saldo = await _repositorio.SomarPontosAsync(1);

            // Assert
            Assert.Equal(0, saldo);
        }

        [Fact]
        [Trait("Repository", "LancamentoPontos")]
        public async Task ListarPorResponsavelAsync_VariasConsultas_RetornaDaMaisRecenteParaAMaisAntiga()
        {
            // Arrange
            _contexto.LancamentosPontos.AddRange(
                Lancamento(1, 1, 30, new DateTime(2026, 9, 1)),
                Lancamento(2, 1, 12, new DateTime(2026, 10, 1)));
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarPorResponsavelAsync(1);

            // Assert
            Assert.Collection(resultado,
                lancamento => Assert.Equal(2, lancamento.ConsultaId),
                lancamento => Assert.Equal(1, lancamento.ConsultaId));
        }

        [Fact]
        [Trait("Repository", "LancamentoPontos")]
        public async Task ObterPorConsultaAsync_ConsultaPontuada_RetornaOLancamento()
        {
            // Arrange
            await _repositorio.AdicionarAsync(Lancamento(42, 1, 33, new DateTime(2026, 10, 1)));

            // Act
            var resultado = await _repositorio.ObterPorConsultaAsync(42);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(33, resultado!.Pontos);
        }

        [Fact]
        [Trait("Repository", "LancamentoPontos")]
        public async Task ObterPorConsultaAsync_ConsultaNaoPontuada_RetornaNull()
        {
            // Act
            var resultado = await _repositorio.ObterPorConsultaAsync(999);

            // Assert
            Assert.Null(resultado);
        }
    }
}
