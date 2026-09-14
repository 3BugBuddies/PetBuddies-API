using System.Net;
using Moq;
using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Presentation;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.App
{
    public class LimiteDeRequisicoesTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public LimiteDeRequisicoesTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _factory.OfertaServiceMock.Reset();
        }

        [Fact]
        [Trait("LimiteDeRequisicoes", "Oferta")]
        public async Task Listar_AcimaDoLimiteDaJanela_Retorna429ComRetryAfter()
        {
            // Arrange
            _factory.OfertaServiceMock
                .Setup(service => service.ListarAsync(
                    It.IsAny<int?>(), It.IsAny<TipoAtoOfertaEnum?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OfertaDto>());

            using var cliente = _factory.CriarClienteComPerfil("VET");

            for (var i = 0; i < LimiteDeRequisicoes.PermissoesPorJanela; i++)
            {
                var permitida = await cliente.GetAsync("/api/oferta");
                Assert.NotEqual(HttpStatusCode.TooManyRequests, permitida.StatusCode);
            }

            // Act
            var resposta = await cliente.GetAsync("/api/oferta");

            // Assert
            Assert.Equal(HttpStatusCode.TooManyRequests, resposta.StatusCode);
            Assert.True(resposta.Headers.Contains("Retry-After"));
        }
    }
}
