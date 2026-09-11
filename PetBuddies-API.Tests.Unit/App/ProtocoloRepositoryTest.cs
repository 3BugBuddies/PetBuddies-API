using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Repositories;

namespace PetBuddies_API.Tests.Unit.App
{
    public class ProtocoloRepositoryTest
    {
        private readonly ApplicationContext _contexto;
        private readonly ProtocoloRepository _repositorio;

        // Nome único por instância: cada [Fact] instancia a classe de novo (xUnit), então
        // cada teste já nasce com banco próprio — sem paralelismo compartilhando estado.
        public ProtocoloRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new ApplicationContext(options);
            _repositorio = new ProtocoloRepository(_contexto);
        }

        [Fact]
        [Trait("Repository", "Protocolo")]
        public async Task ListarAsync_SemFiltro_RetornaTodosOrdenadosPorNome()
        {
            // Arrange
            var protocolos = new List<ProtocoloEntity>
            {
                new() { Nome = "Vacinação anual", Categoria = CategoriaProtocoloEnum.PREVENTIVO, Especie = EspecieEnum.CACHORRO, Ativo = true },
                new() { Nome = "Castração", Categoria = CategoriaProtocoloEnum.POS_CIRURGICO, Especie = EspecieEnum.GATO, Ativo = true },
                new() { Nome = "Antipulgas", Categoria = CategoriaProtocoloEnum.PREVENTIVO, Especie = EspecieEnum.CACHORRO, Ativo = false }
            };

            _contexto.Protocolos.AddRange(protocolos);
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarAsync();

            // Assert
            Assert.Collection(resultado,
                protocolo => Assert.Equal("Antipulgas", protocolo.Nome),
                protocolo => Assert.Equal("Castração", protocolo.Nome),
                protocolo => Assert.Equal("Vacinação anual", protocolo.Nome));
        }

        [Fact]
        [Trait("Repository", "Protocolo")]
        public async Task ListarAsync_FiltradoPorAtivo_RetornaSoOsAtivos()
        {
            // Arrange
            _contexto.Protocolos.AddRange(
                new ProtocoloEntity { Nome = "Ativo", Categoria = CategoriaProtocoloEnum.PREVENTIVO, Especie = EspecieEnum.CACHORRO, Ativo = true },
                new ProtocoloEntity { Nome = "Inativo", Categoria = CategoriaProtocoloEnum.PREVENTIVO, Especie = EspecieEnum.CACHORRO, Ativo = false });
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarAsync(ativo: true);

            // Assert
            var protocolo = Assert.Single(resultado);
            Assert.Equal("Ativo", protocolo.Nome);
        }

        [Fact]
        [Trait("Repository", "Protocolo")]
        public async Task ObterPorIdAsync_ProtocoloExistente_RetornaComAsRegras()
        {
            // Arrange
            var protocolo = new ProtocoloEntity
            {
                Nome = "Vacinação anual",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.CACHORRO,
                Ativo = true,
                Regras = new List<RegraProtocoloEntity>
                {
                    new()
                    {
                        Tipo = TipoCuidadoEnum.VACINACAO,
                        Nome = "Primeira dose",
                        Offset = 0,
                        UnidadeOffset = UnidadeTempoEnum.DIAS,
                        DataBase = TipoDataBaseEnum.NASCIMENTO,
                        Repeticoes = 1
                    }
                }
            };

            _contexto.Protocolos.Add(protocolo);
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ObterPorIdAsync(protocolo.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(protocolo.Nome, resultado!.Nome);
            Assert.Single(resultado.Regras);
        }

        [Fact]
        [Trait("Repository", "Protocolo")]
        public async Task ExisteAsync_IdInexistente_RetornaFalse()
        {
            // Act
            var existe = await _repositorio.ExisteAsync(long.MaxValue);

            // Assert
            Assert.False(existe);
        }

        [Fact]
        [Trait("Repository", "Protocolo")]
        public async Task AdicionarAsync_Protocolo_PersisteNoBanco()
        {
            // Arrange
            var protocolo = new ProtocoloEntity
            {
                Nome = "Protocolo novo",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.GATO,
                Ativo = true
            };

            // Act
            await _repositorio.AdicionarAsync(protocolo);
            await _contexto.SaveChangesAsync();

            // Assert
            var noBanco = await _contexto.Protocolos.FindAsync(protocolo.Id);
            Assert.NotNull(noBanco);
            Assert.Equal("Protocolo novo", noBanco!.Nome);
        }

        [Fact]
        [Trait("Repository", "Protocolo")]
        public async Task Remover_ProtocoloExistente_RemoveDoBanco()
        {
            // Arrange
            var protocolo = new ProtocoloEntity
            {
                Nome = "Para remover",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.CACHORRO,
                Ativo = true
            };
            _contexto.Protocolos.Add(protocolo);
            await _contexto.SaveChangesAsync();

            // Act
            await _repositorio.RemoverAsync(protocolo);
            await _contexto.SaveChangesAsync();

            // Assert
            Assert.False(await _contexto.Protocolos.AnyAsync(p => p.Id == protocolo.Id));
        }
    }
}
