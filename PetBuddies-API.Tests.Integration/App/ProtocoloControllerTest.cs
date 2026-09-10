using System.Net;
using System.Net.Http.Json;
using Moq;
using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.App
{
    public class ProtocoloControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public ProtocoloControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;

            // Reseta setups do teste anterior: o mock é compartilhado pela classe inteira
            // via IClassFixture, e Moq não limpa configuração entre [Fact]s sozinho.
            _factory.ProtocoloServiceMock.Reset();
        }

        private static SalvarProtocoloRequest ProtocoloValido() => new()
        {
            Nome = "Vacinação anual",
            Categoria = CategoriaProtocoloEnum.PREVENTIVO,
            Especie = EspecieEnum.CACHORRO,
            Ativo = true
        };

        [Fact]
        [Trait("Controller", "Protocolo")]
        public async Task Listar_SemProtocolos_Retorna204()
        {
            // Arrange
            _factory.ProtocoloServiceMock
                .Setup(service => service.ListarAsync(null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProtocoloDto>());

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/protocolos");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Protocolo")]
        public async Task BuscarPorId_ProtocoloExistente_Retorna200ComODto()
        {
            // Arrange
            var dto = new ProtocoloDto
            {
                Id = 7,
                Nome = "Vacinação anual",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.CACHORRO,
                Ativo = true
            };

            _factory.ProtocoloServiceMock
                .Setup(service => service.BuscarPorIdAsync(7, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/protocolos/7");
            var encontrado = await resposta.Content.ReadFromJsonAsync<ProtocoloDto>(JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal("Vacinação anual", encontrado!.Nome);
        }

        [Fact]
        [Trait("Controller", "Protocolo")]
        public async Task BuscarPorId_ProtocoloInexistente_Retorna404()
        {
            // Arrange
            _factory.ProtocoloServiceMock
                .Setup(service => service.BuscarPorIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProtocoloDto?)null);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/protocolos/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Protocolo")]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var dto = new ProtocoloDto
            {
                Id = 1,
                Nome = "Vacinação anual",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.CACHORRO,
                Ativo = true
            };

            _factory.ProtocoloServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarProtocoloRequest>()))
                .Returns((string?)null);
            _factory.ProtocoloServiceMock
                .Setup(service => service.CadastrarAsync(It.IsAny<SalvarProtocoloRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/protocolos", ProtocoloValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Protocolo")]
        public async Task Cadastrar_RequestInvalido_Retorna400()
        {
            // Arrange
            _factory.ProtocoloServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarProtocoloRequest>()))
                .Returns("Nome do protocolo é obrigatório.");

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/protocolos", ProtocoloValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }
    }
}
