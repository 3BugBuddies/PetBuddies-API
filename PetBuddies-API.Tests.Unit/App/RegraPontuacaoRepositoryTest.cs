using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Repositories;

namespace PetBuddies_API.Tests.Unit.App
{
    public class RegraPontuacaoRepositoryTest
    {
        private readonly ApplicationContext _contexto;
        private readonly RegraPontuacaoRepository _repositorio;

        public RegraPontuacaoRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new ApplicationContext(options);
            _repositorio = new RegraPontuacaoRepository(_contexto);
        }

        private static RegraPontuacaoEntity Regra(int clinicaId, DateOnly inicioVigencia, TipoGestoEnum gesto = TipoGestoEnum.CONSULTA_REALIZADA) => new()
        {
            ClinicaId = clinicaId,
            Gesto = gesto,
            Pontos = 10,
            InicioVigencia = inicioVigencia
        };

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task ListarAsync_SemFiltro_RetornaOrdenadoPorVigenciaDescendente()
        {
            // Arrange
            _contexto.RegrasPontuacao.AddRange(
                Regra(1, new DateOnly(2026, 1, 1)),
                Regra(1, new DateOnly(2026, 6, 1)));
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarAsync();

            // Assert
            Assert.Collection(resultado,
                regra => Assert.Equal(new DateOnly(2026, 6, 1), regra.InicioVigencia),
                regra => Assert.Equal(new DateOnly(2026, 1, 1), regra.InicioVigencia));
        }

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task ListarAsync_FiltradoPorGesto_RetornaSoAsCorrespondentes()
        {
            // Arrange
            _contexto.RegrasPontuacao.AddRange(
                Regra(1, new DateOnly(2026, 1, 1), TipoGestoEnum.CONSULTA_REALIZADA),
                Regra(1, new DateOnly(2026, 1, 2), TipoGestoEnum.PLANO_CRIADO));
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarAsync(gesto: TipoGestoEnum.PLANO_CRIADO);

            // Assert
            var regra = Assert.Single(resultado);
            Assert.Equal(TipoGestoEnum.PLANO_CRIADO, regra.Gesto);
        }

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task ObterPorIdAsync_RegraExistente_RetornaARegra()
        {
            // Arrange
            var regra = Regra(1, new DateOnly(2026, 1, 1));
            _contexto.RegrasPontuacao.Add(regra);
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ObterPorIdAsync(regra.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(10, resultado!.Pontos);
        }

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task VigenciaExisteAsync_MesmaClinicaGestoEVigencia_RetornaTrue()
        {
            // Arrange — espelha UK_PONTUACAO_VIGENCIA.
            var vigencia = new DateOnly(2026, 3, 1);
            _contexto.RegrasPontuacao.Add(Regra(1, vigencia));
            await _contexto.SaveChangesAsync();

            // Act
            var existe = await _repositorio.VigenciaExisteAsync(1, TipoGestoEnum.CONSULTA_REALIZADA, vigencia);

            // Assert
            Assert.True(existe);
        }

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task VigenciaExisteAsync_IgnorandoAPropriaRegra_RetornaFalse()
        {
            // Arrange — caso de uso da atualização: a regra não conflita consigo mesma.
            var vigencia = new DateOnly(2026, 3, 1);
            var regra = Regra(1, vigencia);
            _contexto.RegrasPontuacao.Add(regra);
            await _contexto.SaveChangesAsync();

            // Act
            var existe = await _repositorio.VigenciaExisteAsync(
                1, TipoGestoEnum.CONSULTA_REALIZADA, vigencia, ignorarRegraId: regra.Id);

            // Assert
            Assert.False(existe);
        }

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task AdicionarAsync_Regra_PersisteNoBanco()
        {
            // Arrange
            var regra = Regra(1, new DateOnly(2026, 1, 1));

            // Act
            await _repositorio.AdicionarAsync(regra);
            await _contexto.SaveChangesAsync();

            // Assert
            var noBanco = await _contexto.RegrasPontuacao.FindAsync(regra.Id);
            Assert.NotNull(noBanco);
            Assert.Equal(10, noBanco!.Pontos);
        }

        [Fact]
        [Trait("Repository", "RegraPontuacao")]
        public async Task Remover_RegraExistente_RemoveDoBanco()
        {
            // Arrange
            var regra = Regra(1, new DateOnly(2026, 1, 1));
            _contexto.RegrasPontuacao.Add(regra);
            await _contexto.SaveChangesAsync();

            // Act
            _repositorio.Remover(regra);
            await _contexto.SaveChangesAsync();

            // Assert
            Assert.False(await _contexto.RegrasPontuacao.AnyAsync(r => r.Id == regra.Id));
        }
    }
}
