using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PetBuddies_API.Application.UseCases;
using PetBuddies_API.Domain.Documents;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Domain.Interfaces;
using PetBuddies_API.Tests.Unit.Fixtures;

namespace PetBuddies_API.Tests.Unit.App
{
    [Collection(ServicosDoBackOfficeCollection.NomeDaColecao)]
    public class PontuacaoServiceTest
    {
        private readonly RequestBuilderFixture _fixture;
        private readonly Mock<ILancamentoPontosRepository> _lancamentosMock = new();
        private readonly Mock<IRegraPontuacaoRepository> _regrasMock = new();
        private readonly Mock<IMemoriaCalculoRepository> _memoriasMock = new();
        private readonly PontuacaoService _service;

        public PontuacaoServiceTest(RequestBuilderFixture fixture)
        {
            _fixture = fixture;
            _service = new PontuacaoService(
                _lancamentosMock.Object,
                _regrasMock.Object,
                _memoriasMock.Object,
                NullLogger<PontuacaoService>.Instance);

            _regrasMock
                .Setup(repositorio => repositorio.ListarVigentesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RegraPontuacaoEntity>
                {
                    new() { Id = 1, ClinicaId = 1, Gesto = TipoGestoEnum.CONSULTA_REALIZADA, Pontos = 10, InicioVigencia = new DateOnly(2026, 1, 1) }
                });
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public void Validar_RequestValido_RetornaNull()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida();

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public void Validar_CheckinsAcimaDasDosesPrevistas_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida() with { DosesPrevistas = 5, DosesComCheckin = 6 };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public void Validar_ConsultaAnteriorDepoisDaAtual_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida() with { DataConsultaAnterior = new DateTime(2026, 11, 1) };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public void Validar_ContagemNegativa_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida() with { DiasNoPeriodo = -1 };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public void Validar_SemConsultaId_RetornaMensagemDeErro()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida() with { ConsultaId = null };

            // Act
            var resultado = _service.Validar(request);

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public async Task PontuarConsultaAsync_ConsultaJaPontuada_DevolveOExistenteSemGravar()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida();
            _lancamentosMock
                .Setup(repositorio => repositorio.ObterPorConsultaAsync(request.ConsultaId!.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LancamentoPontosEntity { Id = 7, ConsultaId = request.ConsultaId!.Value, Pontos = 33 });

            // Act
            var resultado = await _service.PontuarConsultaAsync(request);

            // Assert
            Assert.True(resultado.JaExistia);
            Assert.Equal(33, resultado.Lancamento.Pontos);
            _lancamentosMock.Verify(
                repositorio => repositorio.AdicionarAsync(It.IsAny<LancamentoPontosEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _memoriasMock.Verify(
                repositorio => repositorio.SalvarAsync(It.IsAny<MemoriaCalculoDocument>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public async Task PontuarConsultaAsync_ConsultaNova_GravaLancamentoEMemoriaComOMesmoTotal()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida();
            MemoriaCalculoDocument? memoriaGravada = null;
            _memoriasMock
                .Setup(repositorio => repositorio.SalvarAsync(It.IsAny<MemoriaCalculoDocument>(), It.IsAny<CancellationToken>()))
                .Callback<MemoriaCalculoDocument, CancellationToken>((memoria, _) => memoriaGravada = memoria)
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.PontuarConsultaAsync(request);

            // Assert — só CONSULTA_REALIZADA tem regra: 10 pontos.
            Assert.False(resultado.JaExistia);
            Assert.Equal(10, resultado.Lancamento.Pontos);
            _lancamentosMock.Verify(
                repositorio => repositorio.AdicionarAsync(It.IsAny<LancamentoPontosEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.NotNull(memoriaGravada);
            Assert.Equal(10, memoriaGravada!.Total);
            Assert.Equal(request.DataConsultaAnterior, memoriaGravada.Periodo.De);
            Assert.Equal(4, memoriaGravada.Parcelas.Count);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public async Task PontuarConsultaAsync_MongoForaDoAr_MantemOLancamento()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida();
            _memoriasMock
                .Setup(repositorio => repositorio.SalvarAsync(It.IsAny<MemoriaCalculoDocument>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("MongoDB indisponível"));

            // Act
            var resultado = await _service.PontuarConsultaAsync(request);

            // Assert
            Assert.False(resultado.JaExistia);
            Assert.Equal(10, resultado.Lancamento.Pontos);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public async Task PontuarConsultaAsync_DataDaConsulta_BuscaRegrasVigentesNessaData()
        {
            // Arrange
            var request = _fixture.PontuarConsultaValida();

            // Act
            await _service.PontuarConsultaAsync(request);

            // Assert
            _regrasMock.Verify(
                repositorio => repositorio.ListarVigentesAsync(1, new DateOnly(2026, 10, 1), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        [Trait("Service", "Pontuacao")]
        public async Task ObterSaldoAsync_ResponsavelInformado_DevolveASomaDoRepositorio()
        {
            // Arrange
            _lancamentosMock
                .Setup(repositorio => repositorio.SomarPontosAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(120);

            // Act
            var saldo = await _service.ObterSaldoAsync(1);

            // Assert
            Assert.Equal(1, saldo.ResponsavelId);
            Assert.Equal(120, saldo.Saldo);
        }
    }
}
