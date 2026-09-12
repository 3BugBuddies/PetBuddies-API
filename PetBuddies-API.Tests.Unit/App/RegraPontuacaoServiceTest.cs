using Moq;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Tests.Unit.Fixtures;

namespace PetBuddies_API.Tests.Unit.App
{
    [Collection(ServicosDoBackOfficeCollection.NomeDaColecao)]
    public class RegraPontuacaoServiceTest
    {
        private readonly RequestBuilderFixture _fixture;
        private readonly Mock<IRegraPontuacaoRepository> _repositorioMock = new();
        private readonly RegraPontuacaoService _service;

        public RegraPontuacaoServiceTest(RequestBuilderFixture fixture)
        {
            _fixture = fixture;
            _service = new RegraPontuacaoService(_repositorioMock.Object);
        }

        [Fact]
        [Trait("Service", "RegraPontuacao")]
        public void Validar_RequestValido_RetornaNull()
        {
            // Arrange
            var request = _fixture.RegraPontuacaoValida();

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "RegraPontuacao")]
        public void Validar_PontosNulo_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.RegraPontuacaoValida() with { Pontos = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [Trait("Service", "RegraPontuacao")]
        public void Validar_PontosMenorOuIgualAZero_RetornaMensagemDeErro(int pontos)
        {
            // Arrange — CK_PONTUACAO_PONTOS: NR_PONTOS > 0.
            var request = _fixture.RegraPontuacaoValida() with { Pontos = pontos };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraPontuacao")]
        public void Validar_GestoForaDoEnum_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.RegraPontuacaoValida() with { Gesto = (TipoGestoEnum)999 };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraPontuacao")]
        public void Validar_InicioVigenciaNulo_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.RegraPontuacaoValida() with { InicioVigencia = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraPontuacao")]
        public async Task CadastrarAsync_RequestValido_ChamaAdicionarAsyncExatamenteUmaVez()
        {
            // Arrange
            var request = _fixture.RegraPontuacaoValida();

            // Act
            await _service.CadastrarAsync(request);

            // Assert
            _repositorioMock.Verify(
                repositorio => repositorio.AdicionarAsync(It.IsAny<RegraPontuacaoEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        [Trait("Service", "RegraPontuacao")]
        public async Task VigenciaExisteAsync_ParametrosInformados_DelegaAoRepositorio()
        {
            // Arrange — UK_PONTUACAO_VIGENCIA: um valor por clínica, gesto e vigência.
            var request = _fixture.RegraPontuacaoValida();
            _repositorioMock
                .Setup(repositorio => repositorio.VigenciaExisteAsync(
                    request.ClinicaId,
                    request.Gesto!.Value,
                    request.InicioVigencia!.Value,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var existe = await _service.VigenciaExisteAsync(request);

            // Assert
            Assert.True(existe);
        }
    }
}
