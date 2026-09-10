using System.Net;
using System.Net.Http.Json;
using Moq;
using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.App
{
    public class OfertaControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public OfertaControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _factory.OfertaServiceMock.Reset();
        }

        private static SalvarOfertaRequest OfertaValida() => new()
        {
            ClinicaId = 1,
            Ato = TipoAtoOfertaEnum.CONSULTA,
            Subtipo = "CLINICA_GERAL",
            Descricao = "Consulta de rotina",
            Valor = 120m,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        [Fact]
        [Trait("Controller", "Oferta")]
        public async Task BuscarPorId_OfertaInexistente_Retorna404()
        {
            // Arrange
            _factory.OfertaServiceMock
                .Setup(service => service.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((OfertaDto?)null);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/oferta/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Oferta")]
        public async Task Cadastrar_RequestInvalido_Retorna400()
        {
            // Arrange
            _factory.OfertaServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarOfertaRequest>()))
                .Returns("Valor da oferta não pode ser negativo.");

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/oferta", OfertaValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Oferta")]
        public async Task Cadastrar_AtoProtocoloComProtocoloInexistente_Retorna404()
        {
            // Arrange
            var request = OfertaValida() with { Ato = TipoAtoOfertaEnum.PROTOCOLO, Subtipo = null, ProtocoloId = 1 };

            _factory.OfertaServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarOfertaRequest>()))
                .Returns((string?)null);
            _factory.OfertaServiceMock
                .Setup(service => service.ProtocoloExisteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/oferta", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Oferta")]
        public async Task Cadastrar_VigenciaJaExiste_Retorna409()
        {
            // Arrange — espelha UX_OFERTA_VIGENCIA, aqui só a decisão do controller.
            _factory.OfertaServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarOfertaRequest>()))
                .Returns((string?)null);
            _factory.OfertaServiceMock
                .Setup(service => service.VigenciaExisteAsync(It.IsAny<SalvarOfertaRequest>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/oferta", OfertaValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Oferta")]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var dto = new OfertaDto { Id = 1, ClinicaId = 1, Ato = TipoAtoOfertaEnum.CONSULTA, Subtipo = "CLINICA_GERAL", Descricao = "Consulta de rotina", Valor = 120m, InicioVigencia = new DateOnly(2026, 1, 1) };

            _factory.OfertaServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarOfertaRequest>()))
                .Returns((string?)null);
            _factory.OfertaServiceMock
                .Setup(service => service.VigenciaExisteAsync(It.IsAny<SalvarOfertaRequest>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _factory.OfertaServiceMock
                .Setup(service => service.CadastrarAsync(It.IsAny<SalvarOfertaRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/oferta", OfertaValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }
    }
}
