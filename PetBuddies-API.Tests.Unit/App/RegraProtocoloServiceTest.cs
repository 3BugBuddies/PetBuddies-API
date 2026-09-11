using Moq;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Tests.Unit.Fixtures;

namespace PetBuddies_API.Tests.Unit.App
{
    [Collection(ServicosDoBackOfficeCollection.NomeDaColecao)]
    public class RegraProtocoloServiceTest
    {
        private readonly RequestBuilderFixture _fixture;
        private readonly Mock<IRegraProtocoloRepository> _repositorioMock = new();
        private readonly Mock<IProtocoloRepository> _protocoloRepositorioMock = new();
        private readonly RegraProtocoloService _service;

        public RegraProtocoloServiceTest(RequestBuilderFixture fixture)
        {
            _fixture = fixture;
            _service = new RegraProtocoloService(
                _repositorioMock.Object,
                _protocoloRepositorioMock.Object);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_RequestValido_RetornaNull()
        {
            // Arrange
            var request = _fixture.RegraProtocoloValida();

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_RepeticoesNulo_RetornaMensagemDeErro()
        {
            // Arrange — NR_REPETICOES é NOT NULL: nulo geraria laço infinito no motor.
            var request = _fixture.RegraProtocoloValida() with { Repeticoes = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_RepeticoesMenorQueUm_RetornaMensagemDeErro(int repeticoes)
        {
            // Arrange — CK_REGPROT_REPETICOES exige >= 1.
            var request = _fixture.RegraProtocoloValida() with { Repeticoes = repeticoes };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_IntervaloPreenchidoSemUnidade_RetornaMensagemDeErro()
        {
            // Arrange — CK_REGPROT_RECORRENCIA: os dois vêm juntos, ou nenhum.
            var request = _fixture.RegraProtocoloValida() with { Intervalo = 30, UnidadeIntervalo = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_UnidadeIntervaloPreenchidaSemIntervalo_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.RegraProtocoloValida() with
            {
                Intervalo = null,
                UnidadeIntervalo = UnidadeTempoEnum.DIAS
            };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_IntervaloEUnidadeAmbosNulos_RetornaNull()
        {
            // Arrange — ocorrência única: os dois nulos é o caso válido.
            var request = _fixture.RegraProtocoloValida() with { Intervalo = null, UnidadeIntervalo = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_IntervaloEUnidadeAmbosPreenchidos_RetornaNull()
        {
            // Arrange — regra recorrente: os dois preenchidos é o outro caso válido.
            var request = _fixture.RegraProtocoloValida() with
            {
                Intervalo = 30,
                UnidadeIntervalo = UnidadeTempoEnum.DIAS
            };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public void Validar_TipoDeCuidadoForaDoEnum_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.RegraProtocoloValida() with { Tipo = (TipoCuidadoEnum)999 };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public async Task CadastrarAsync_RequestValido_ChamaAdicionarAsyncExatamenteUmaVez()
        {
            // Arrange
            var request = _fixture.RegraProtocoloValida();

            // Act
            await _service.CadastrarAsync(request);

            // Assert
            _repositorioMock.Verify(
                repositorio => repositorio.AdicionarAsync(It.IsAny<RegraProtocoloEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        [Trait("Service", "RegraProtocolo")]
        public async Task ProtocoloExisteAsync_DelegaParaRepositorioDeProtocolo()
        {
            // Arrange
            _protocoloRepositorioMock
                .Setup(repositorio => repositorio.ExisteAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var existe = await _service.ProtocoloExisteAsync(5);

            // Assert
            Assert.True(existe);
            _protocoloRepositorioMock.Verify(repositorio => repositorio.ExisteAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
