using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Repositories;

namespace PetBuddies_API.Tests.Unit.App
{
    public class OfertaRepositoryTest
    {
        private readonly ApplicationContext _contexto;
        private readonly OfertaRepository _repositorio;

        // Ofertas de CONSULTA (Subtipo preenchido, sem ProtocoloId): evita depender de um
        // ProtocoloEntity só para satisfazer a FK de ID_PROTOCOLO neste conjunto de testes.
        public OfertaRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new ApplicationContext(options);
            _repositorio = new OfertaRepository(_contexto);
        }

        private static OfertaEntity OfertaDeConsulta(int clinicaId, DateOnly inicioVigencia, string subtipo = "CLINICA_GERAL") => new()
        {
            ClinicaId = clinicaId,
            Ato = TipoAtoOfertaEnum.CONSULTA,
            Subtipo = subtipo,
            Descricao = "Consulta de rotina",
            Valor = 120m,
            InicioVigencia = inicioVigencia
        };

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task ListarAsync_SemFiltro_RetornaOrdenadoPorVigenciaDescendente()
        {
            // Arrange
            _contexto.Ofertas.AddRange(
                OfertaDeConsulta(1, new DateOnly(2026, 1, 1)),
                OfertaDeConsulta(1, new DateOnly(2026, 6, 1)));
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarAsync();

            // Assert
            Assert.Collection(resultado,
                oferta => Assert.Equal(new DateOnly(2026, 6, 1), oferta.InicioVigencia),
                oferta => Assert.Equal(new DateOnly(2026, 1, 1), oferta.InicioVigencia));
        }

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task ListarAsync_FiltradoPorClinicaEAto_RetornaSoAsCorrespondentes()
        {
            // Arrange
            _contexto.Ofertas.AddRange(
                OfertaDeConsulta(1, new DateOnly(2026, 1, 1)),
                OfertaDeConsulta(2, new DateOnly(2026, 1, 1)));
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarAsync(clinicaId: 1, ato: TipoAtoOfertaEnum.CONSULTA);

            // Assert
            var oferta = Assert.Single(resultado);
            Assert.Equal(1, oferta.ClinicaId);
        }

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task ObterPorIdAsync_OfertaExistente_RetornaAOferta()
        {
            // Arrange
            var oferta = OfertaDeConsulta(1, new DateOnly(2026, 1, 1));
            _contexto.Ofertas.Add(oferta);
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ObterPorIdAsync(oferta.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(oferta.Descricao, resultado!.Descricao);
        }

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task VigenciaExisteAsync_MesmoAlvoEVigencia_RetornaTrue()
        {
            // Arrange — espelha UX_OFERTA_VIGENCIA.
            var vigencia = new DateOnly(2026, 3, 1);
            _contexto.Ofertas.Add(OfertaDeConsulta(1, vigencia));
            await _contexto.SaveChangesAsync();

            // Act
            var existe = await _repositorio.VigenciaExisteAsync(1, TipoAtoOfertaEnum.CONSULTA, "CLINICA_GERAL", null, vigencia);

            // Assert
            Assert.True(existe);
        }

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task VigenciaExisteAsync_IgnorandoAPropriaOferta_RetornaFalse()
        {
            // Arrange — é o caso de uso da atualização: a oferta não conflita consigo mesma.
            var vigencia = new DateOnly(2026, 3, 1);
            var oferta = OfertaDeConsulta(1, vigencia);
            _contexto.Ofertas.Add(oferta);
            await _contexto.SaveChangesAsync();

            // Act
            var existe = await _repositorio.VigenciaExisteAsync(
                1, TipoAtoOfertaEnum.CONSULTA, "CLINICA_GERAL", null, vigencia, ignorarOfertaId: oferta.Id);

            // Assert
            Assert.False(existe);
        }

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task AdicionarAsync_Oferta_PersisteNoBanco()
        {
            // Arrange
            var oferta = OfertaDeConsulta(1, new DateOnly(2026, 1, 1));

            // Act
            await _repositorio.AdicionarAsync(oferta);
            await _contexto.SaveChangesAsync();

            // Assert
            var noBanco = await _contexto.Ofertas.FindAsync(oferta.Id);
            Assert.NotNull(noBanco);
            Assert.Equal(120m, noBanco!.Valor);
        }

        [Fact]
        [Trait("Repository", "Oferta")]
        public async Task Remover_OfertaExistente_RemoveDoBanco()
        {
            // Arrange
            var oferta = OfertaDeConsulta(1, new DateOnly(2026, 1, 1));
            _contexto.Ofertas.Add(oferta);
            await _contexto.SaveChangesAsync();

            // Act
            await _repositorio.RemoverAsync(oferta);
            await _contexto.SaveChangesAsync();

            // Assert
            Assert.False(await _contexto.Ofertas.AnyAsync(o => o.Id == oferta.Id));
        }
    }
}
