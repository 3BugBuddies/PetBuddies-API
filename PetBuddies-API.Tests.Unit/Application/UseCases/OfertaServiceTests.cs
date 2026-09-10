using Moq;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Tests.Unit.Fixtures;

namespace PetBuddies_API.Tests.Unit.Application.UseCases
{
    [Collection(ServicosDoBackOfficeCollection.NomeDaColecao)]
    public class OfertaServiceTests
    {
        private readonly RequestBuilderFixture _fixture;
        private readonly Mock<IOfertaRepository> _repositorioMock = new();
        private readonly Mock<IProtocoloRepository> _protocoloRepositorioMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly OfertaService _service;

        public OfertaServiceTests(RequestBuilderFixture fixture)
        {
            _fixture = fixture;
            _service = new OfertaService(
                _repositorioMock.Object,
                _protocoloRepositorioMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
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
        public async Task CadastrarAsync_RequestValido_ConfirmaExatamenteUmaVezNoUnitOfWork()
        {
            // Arrange
            var request = _fixture.OfertaDeConsultaValida();

            // Act
            await _service.CadastrarAsync(request);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
