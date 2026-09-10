using System.Net;
using System.Net.Http.Json;
using Moq;
using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.App
{
    public class RegraProtocoloControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public RegraProtocoloControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _factory.RegraProtocoloServiceMock.Reset();
        }

        private static SalvarRegraProtocoloRequest RegraValida(long protocoloId = 1) => new()
        {
            ProtocoloId = protocoloId,
            Tipo = TipoCuidadoEnum.VACINACAO,
            Nome = "Primeira dose",
            Offset = 0,
            UnidadeOffset = UnidadeTempoEnum.DIAS,
            DataBase = TipoDataBaseEnum.NASCIMENTO,
            Repeticoes = 1
        };

        [Fact]
        [Trait("Controller", "RegraProtocolo")]
        public async Task Listar_SemProtocoloId_Retorna400()
        {
            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/regras-protocolo");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraProtocolo")]
        public async Task Listar_ProtocoloSemRegras_Retorna204()
        {
            // Arrange
            _factory.RegraProtocoloServiceMock
                .Setup(service => service.ListarPorProtocoloAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RegraProtocoloDto>());

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/regras-protocolo?protocoloId=1");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraProtocolo")]
        public async Task Cadastrar_ProtocoloInexistente_Retorna404()
        {
            // Arrange
            _factory.RegraProtocoloServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarRegraProtocoloRequest>()))
                .Returns((string?)null);
            _factory.RegraProtocoloServiceMock
                .Setup(service => service.ProtocoloExisteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/regras-protocolo", RegraValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraProtocolo")]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var dto = new RegraProtocoloDto { Id = 1, ProtocoloId = 1, Nome = "Primeira dose", Tipo = TipoCuidadoEnum.VACINACAO, Offset = 0, UnidadeOffset = UnidadeTempoEnum.DIAS, DataBase = TipoDataBaseEnum.NASCIMENTO, Repeticoes = 1 };

            _factory.RegraProtocoloServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarRegraProtocoloRequest>()))
                .Returns((string?)null);
            _factory.RegraProtocoloServiceMock
                .Setup(service => service.ProtocoloExisteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _factory.RegraProtocoloServiceMock
                .Setup(service => service.CadastrarAsync(It.IsAny<SalvarRegraProtocoloRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/regras-protocolo", RegraValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraProtocolo")]
        public async Task Cadastrar_RequestInvalido_Retorna400()
        {
            // Arrange
            _factory.RegraProtocoloServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarRegraProtocoloRequest>()))
                .Returns("Número de repetições é obrigatório.");

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/regras-protocolo", RegraValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }
    }
}
