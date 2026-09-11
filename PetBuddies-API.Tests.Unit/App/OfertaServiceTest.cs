using Moq;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Tests.Unit.Fixtures;

namespace PetBuddies_API.Tests.Unit.App
{
    [Collection(ServicosDoBackOfficeCollection.NomeDaColecao)]
    public class OfertaServiceTest
    {
        private readonly RequestBuilderFixture _fixture;
        private readonly Mock<IOfertaRepository> _repositorioMock = new();
        private readonly Mock<IProtocoloRepository> _protocoloRepositorioMock = new();
        private readonly OfertaService _service;

        public OfertaServiceTest(RequestBuilderFixture fixture)
        {
            _fixture = fixture;
            _service = new OfertaService(
                _repositorioMock.Object,
                _protocoloRepositorioMock.Object);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_OfertaDeProtocoloValida_RetornaNull()
        {
            // Arrange
            var request = _fixture.OfertaDeProtocoloValida();

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_OfertaDeConsultaValida_RetornaNull()
        {
            // Arrange
            var request = _fixture.OfertaDeConsultaValida();

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_AtoProtocoloSemProtocoloId_RetornaMensagemDeErro()
        {
            // Arrange — CK_OFERTA_ALVO: PROTOCOLO exige ID_PROTOCOLO.
            var request = _fixture.OfertaDeProtocoloValida() with { ProtocoloId = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_AtoProtocoloComSubtipo_RetornaMensagemDeErro()
        {
            // Arrange — o alvo de uma oferta de PROTOCOLO é a linha do catálogo, não um subtipo.
            var request = _fixture.OfertaDeProtocoloValida() with { Subtipo = "CLINICA_GERAL" };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_AtoConsultaSemSubtipo_RetornaMensagemDeErro()
        {
            // Arrange — CK_OFERTA_ALVO: CONSULTA/PROCEDIMENTO exigem TP_SUBTIPO.
            var request = _fixture.OfertaDeConsultaValida() with { Subtipo = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_AtoConsultaComProtocoloId_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.OfertaDeConsultaValida() with { ProtocoloId = 1 };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        [Trait("Service", "Oferta")]
        public void Validar_ValorNegativo_RetornaMensagemDeErro(decimal valor)
        {
            // Arrange — CK_OFERTA_VALOR: NR_VALOR >= 0.
            var request = _fixture.OfertaDeConsultaValida() with { Valor = valor };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_ValorZero_RetornaNull()
        {
            // Arrange — zero é a fronteira aceita pelo CHECK (>= 0, não > 0).
            var request = _fixture.OfertaDeConsultaValida() with { Valor = 0m };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public void Validar_DescricaoEmBranco_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.OfertaDeConsultaValida() with { Descricao = "  " };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Oferta")]
        public async Task CadastrarAsync_RequestValido_ChamaAdicionarAsyncExatamenteUmaVez()
        {
            // Arrange
            var request = _fixture.OfertaDeConsultaValida();

            // Act
            await _service.CadastrarAsync(request);

            // Assert
            _repositorioMock.Verify(
                repositorio => repositorio.AdicionarAsync(It.IsAny<OfertaEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
