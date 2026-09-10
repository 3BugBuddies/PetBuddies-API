using Moq;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Tests.Unit.Fixtures;

namespace PetBuddies_API.Tests.Unit.App
{
    [Collection(ServicosDoBackOfficeCollection.NomeDaColecao)]
    public class ProtocoloServiceTest
    {
        private readonly RequestBuilderFixture _fixture;
        private readonly Mock<IProtocoloRepository> _repositorioMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly ProtocoloService _service;

        public ProtocoloServiceTest(RequestBuilderFixture fixture)
        {
            _fixture = fixture;
            _service = new ProtocoloService(_repositorioMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        [Trait("Service", "Protocolo")]
        public void Validar_RequestValido_RetornaNull()
        {
            // Arrange
            var request = _fixture.ProtocoloValido();

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Theory]
        [Trait("Service", "Protocolo")]
        [InlineData(null)]
        [InlineData((CategoriaProtocoloEnum)999)]
        public void Validar_CategoriaInvalida_RetornaMensagemDeErro(CategoriaProtocoloEnum? categoria)
        {
            // Arrange — CK_PROTOCOLO_CATEGORIA só aceita PREVENTIVO/POS_CIRURGICO, e nula também é inválida.
            var request = _fixture.ProtocoloValido() with { Categoria = categoria };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Protocolo")]
        public void Validar_EspecieForaDoEnum_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.ProtocoloValido() with { Especie = (EspecieEnum)999 };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Protocolo")]
        public void Validar_NomeEmBranco_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.ProtocoloValido() with { Nome = "   " };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Protocolo")]
        public async Task CadastrarAsync_RequestValido_ConfirmaExatamenteUmaVezNoUnitOfWork()
        {
            // Arrange
            var request = _fixture.ProtocoloValido();

            // Act
            await _service.CadastrarAsync(request);

            // Assert
            _repositorioMock.Verify(
                repositorio => repositorio.AdicionarAsync(It.IsAny<ProtocoloEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        [Trait("Service", "Protocolo")]
        public async Task RemoverAsync_ProtocoloInexistente_RetornaFalseSemConfirmarNoUnitOfWork()
        {
            // Arrange
            _repositorioMock
                .Setup(repositorio => repositorio.ObterParaAlterarAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProtocoloEntity?)null);

            // Act
            var removido = await _service.RemoverAsync(1);

            // Assert
            Assert.False(removido);
            _unitOfWorkMock.Verify(uow => uow.SalvarAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
