using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Repositories;

namespace PetBuddies_API.Tests.Unit.App
{
    public class RegraProtocoloRepositoryTest
    {
        private readonly ApplicationContext _contexto;
        private readonly RegraProtocoloRepository _repositorio;

        public RegraProtocoloRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new ApplicationContext(options);
            _repositorio = new RegraProtocoloRepository(_contexto);
        }

        private async Task<ProtocoloEntity> CriarProtocoloAsync()
        {
            var protocolo = new ProtocoloEntity
            {
                Nome = "Protocolo base",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.CACHORRO,
                Ativo = true
            };

            _contexto.Protocolos.Add(protocolo);
            await _contexto.SaveChangesAsync();
            return protocolo;
        }

        [Fact]
        [Trait("Repository", "RegraProtocolo")]
        public async Task ListarPorProtocoloAsync_RegrasDeVariosProtocolos_RetornaSoAsDoProtocoloOrdenadasPorId()
        {
            // Arrange
            var protocolo = await CriarProtocoloAsync();
            var outroProtocolo = await CriarProtocoloAsync();

            _contexto.RegrasProtocolo.AddRange(
                new RegraProtocoloEntity { ProtocoloId = protocolo.Id, Tipo = TipoCuidadoEnum.VACINACAO, Nome = "Primeira dose", Offset = 0, UnidadeOffset = UnidadeTempoEnum.DIAS, DataBase = TipoDataBaseEnum.NASCIMENTO, Repeticoes = 1 },
                new RegraProtocoloEntity { ProtocoloId = protocolo.Id, Tipo = TipoCuidadoEnum.VACINACAO, Nome = "Reforço", Offset = 30, UnidadeOffset = UnidadeTempoEnum.DIAS, DataBase = TipoDataBaseEnum.NASCIMENTO, Repeticoes = 1 },
                new RegraProtocoloEntity { ProtocoloId = outroProtocolo.Id, Tipo = TipoCuidadoEnum.VERMIFUGACAO, Nome = "De outro protocolo", Offset = 0, UnidadeOffset = UnidadeTempoEnum.DIAS, DataBase = TipoDataBaseEnum.NASCIMENTO, Repeticoes = 1 });
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ListarPorProtocoloAsync(protocolo.Id);

            // Assert
            Assert.Collection(resultado,
                regra => Assert.Equal("Primeira dose", regra.Nome),
                regra => Assert.Equal("Reforço", regra.Nome));
        }

        [Fact]
        [Trait("Repository", "RegraProtocolo")]
        public async Task ObterPorIdAsync_RegraExistente_RetornaARegra()
        {
            // Arrange
            var protocolo = await CriarProtocoloAsync();
            var regra = new RegraProtocoloEntity
            {
                ProtocoloId = protocolo.Id,
                Tipo = TipoCuidadoEnum.VACINACAO,
                Nome = "Primeira dose",
                Offset = 0,
                UnidadeOffset = UnidadeTempoEnum.DIAS,
                DataBase = TipoDataBaseEnum.NASCIMENTO,
                Repeticoes = 1
            };
            _contexto.RegrasProtocolo.Add(regra);
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _repositorio.ObterPorIdAsync(regra.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Primeira dose", resultado!.Nome);
            Assert.Equal(protocolo.Id, resultado.ProtocoloId);
        }

        [Fact]
        [Trait("Repository", "RegraProtocolo")]
        public async Task AdicionarAsync_Regra_PersisteNoBanco()
        {
            // Arrange
            var protocolo = await CriarProtocoloAsync();
            var regra = new RegraProtocoloEntity
            {
                ProtocoloId = protocolo.Id,
                Tipo = TipoCuidadoEnum.EXAME,
                Nome = "Exame de rotina",
                Offset = 15,
                UnidadeOffset = UnidadeTempoEnum.DIAS,
                DataBase = TipoDataBaseEnum.ULTIMA_REALIZACAO,
                Repeticoes = 2
            };

            // Act
            await _repositorio.AdicionarAsync(regra);
            await _contexto.SaveChangesAsync();

            // Assert
            var noBanco = await _contexto.RegrasProtocolo.FindAsync(regra.Id);
            Assert.NotNull(noBanco);
            Assert.Equal("Exame de rotina", noBanco!.Nome);
        }

        [Fact]
        [Trait("Repository", "RegraProtocolo")]
        public async Task Remover_RegraExistente_RemoveDoBanco()
        {
            // Arrange
            var protocolo = await CriarProtocoloAsync();
            var regra = new RegraProtocoloEntity
            {
                ProtocoloId = protocolo.Id,
                Tipo = TipoCuidadoEnum.VACINACAO,
                Nome = "Para remover",
                Offset = 0,
                UnidadeOffset = UnidadeTempoEnum.DIAS,
                DataBase = TipoDataBaseEnum.NASCIMENTO,
                Repeticoes = 1
            };
            _contexto.RegrasProtocolo.Add(regra);
            await _contexto.SaveChangesAsync();

            // Act
            await _repositorio.RemoverAsync(regra);
            await _contexto.SaveChangesAsync();

            // Assert
            Assert.False(await _contexto.RegrasProtocolo.AnyAsync(r => r.Id == regra.Id));
        }
    }
}
