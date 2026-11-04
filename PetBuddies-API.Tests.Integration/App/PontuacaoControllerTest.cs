using System.Net;
using System.Net.Http.Json;
using Moq;
using PetBuddies_API.Application.Dtos.Pontuacao;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.App
{
    public class PontuacaoControllerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public PontuacaoControllerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _factory.PontuacaoServiceMock.Reset();
        }

        private static PontuarConsultaRequest RequestValido() => new()
        {
            ConsultaId = 42,
            ResponsavelId = 1,
            AnimalId = 3,
            ClinicaId = 1,
            DataConsulta = new DateTime(2026, 10, 1, 14, 30, 0),
            DosesPrevistas = 20,
            DosesComCheckin = 18,
            DiasNoPeriodo = 30,
            DiasComCheckin = 17,
            AcionamentosClinica = 0,
            Evolucao = EvolucaoClinicaEnum.MELHOROU
        };

        private static LancamentoPontosDto Lancamento() => new()
        {
            Id = 7,
            ConsultaId = 42,
            ResponsavelId = 1,
            AnimalId = 3,
            ClinicaId = 1,
            DataConsulta = new DateTime(2026, 10, 1, 14, 30, 0),
            Pontos = 33
        };

        private static MemoriaCalculoDto Memoria(long responsavelId) => new()
        {
            ConsultaId = 42,
            LancamentoId = 7,
            ResponsavelId = responsavelId,
            Total = 33
        };

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task PontuarConsulta_ConsultaNova_Retorna201()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.Validar(It.IsAny<PontuarConsultaRequest>()))
                .Returns((string?)null);
            _factory.PontuacaoServiceMock
                .Setup(service => service.PontuarConsultaAsync(It.IsAny<PontuarConsultaRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResultadoPontuacaoDto(Lancamento(), JaExistia: false));

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/pontuacao/consulta", RequestValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task PontuarConsulta_ConsultaJaPontuada_Retorna200()
        {
            // Arrange — reenvio do Java: não pontua de novo.
            _factory.PontuacaoServiceMock
                .Setup(service => service.Validar(It.IsAny<PontuarConsultaRequest>()))
                .Returns((string?)null);
            _factory.PontuacaoServiceMock
                .Setup(service => service.PontuarConsultaAsync(It.IsAny<PontuarConsultaRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResultadoPontuacaoDto(Lancamento(), JaExistia: true));

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/pontuacao/consulta", RequestValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task PontuarConsulta_RequestInvalido_Retorna400()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.Validar(It.IsAny<PontuarConsultaRequest>()))
                .Returns("Doses com check-in não podem passar das doses previstas.");

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/pontuacao/consulta", RequestValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task PontuarConsulta_TokenDeTutor_Retorna403()
        {
            // Arrange — só o Java, com o token de serviço (VET), lança pontos.
            using var cliente = _factory.CriarClienteComPerfil("TUTOR", responsavelId: 1);

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/pontuacao/consulta", RequestValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterSaldo_TutorComVinculo_Retorna200ComOSaldoDele()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.ObterSaldoAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SaldoPontosDto { ResponsavelId = 1, Saldo = 120 });

            using var cliente = _factory.CriarClienteComPerfil("TUTOR", responsavelId: 1);

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/saldo");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var saldo = await resposta.Content.ReadFromJsonAsync<SaldoPontosDto>(JsonPadrao.Opcoes);
            Assert.Equal(120, saldo!.Saldo);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterSaldo_TutorSemVinculo_Retorna403()
        {
            // Arrange
            using var cliente = _factory.CriarClienteComPerfil("TUTOR");

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/saldo");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterSaldo_TokenDeVeterinario_Retorna403()
        {
            // Arrange
            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/saldo");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ListarExtrato_SemLancamentos_Retorna204()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.ListarExtratoAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<LancamentoPontosDto>());

            using var cliente = _factory.CriarClienteComPerfil("TUTOR", responsavelId: 1);

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/extrato");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterMemoria_ConsultaDeOutroResponsavel_Retorna403()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.ObterMemoriaAsync(42, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Memoria(responsavelId: 2));

            using var cliente = _factory.CriarClienteComPerfil("TUTOR", responsavelId: 1);

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/consulta/42/memoria");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterMemoria_VeterinarioConsultaQualquerUma_Retorna200()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.ObterMemoriaAsync(42, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Memoria(responsavelId: 2));

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/consulta/42/memoria");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterMemoria_Inexistente_Retorna404()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.ObterMemoriaAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((MemoriaCalculoDto?)null);

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/consulta/999/memoria");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        [Trait("Controller", "Pontuacao")]
        public async Task ObterMemoria_MongoForaDoAr_Retorna503()
        {
            // Arrange
            _factory.PontuacaoServiceMock
                .Setup(service => service.ObterMemoriaAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException());

            using var cliente = _factory.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.GetAsync("/api/pontuacao/consulta/42/memoria");

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, resposta.StatusCode);
        }
    }
}
