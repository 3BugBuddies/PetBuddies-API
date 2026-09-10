using System.Net;
using System.Net.Http.Json;
using Moq;
using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.App
{
    public class RegraPontuacaoControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public RegraPontuacaoControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _factory.RegraPontuacaoServiceMock.Reset();
        }

        private static SalvarRegraPontuacaoRequest RegraValida() => new()
        {
            ClinicaId = 1,
            Gesto = TipoGestoEnum.CONSULTA_REALIZADA,
            Pontos = 10,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        [Fact]
        [Trait("Controller", "RegraPontuacao")]
        public async Task Listar_SemRegras_Retorna204()
        {
            // Arrange
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.ListarAsync(null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RegraPontuacaoDto>());

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/regras-pontuacao");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraPontuacao")]
        public async Task BuscarPorId_RegraInexistente_Retorna404()
        {
            // Arrange
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RegraPontuacaoDto?)null);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/regras-pontuacao/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraPontuacao")]
        public async Task Cadastrar_RequestInvalido_Retorna400()
        {
            // Arrange
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarRegraPontuacaoRequest>()))
                .Returns("Pontos têm de ser positivos.");

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/regras-pontuacao", RegraValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraPontuacao")]
        public async Task Cadastrar_VigenciaJaExiste_Retorna409()
        {
            // Arrange — espelha UK_PONTUACAO_VIGENCIA, aqui só a decisão do controller.
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarRegraPontuacaoRequest>()))
                .Returns((string?)null);
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.VigenciaExisteAsync(It.IsAny<SalvarRegraPontuacaoRequest>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/regras-pontuacao", RegraValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "RegraPontuacao")]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var dto = new RegraPontuacaoDto { Id = 1, ClinicaId = 1, Gesto = TipoGestoEnum.CONSULTA_REALIZADA, Pontos = 10, InicioVigencia = new DateOnly(2026, 1, 1) };

            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.Validar(It.IsAny<SalvarRegraPontuacaoRequest>()))
                .Returns((string?)null);
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.VigenciaExisteAsync(It.IsAny<SalvarRegraPontuacaoRequest>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _factory.RegraPontuacaoServiceMock
                .Setup(service => service.CadastrarAsync(It.IsAny<SalvarRegraPontuacaoRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/regras-pontuacao", RegraValida(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }
    }
}
