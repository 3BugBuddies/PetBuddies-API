using System.Net;
using System.Net.Http.Json;
using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.Controllers
{
    [Collection(PetBuddiesApiCollection.NomeDaColecao)]
    public class RegraPontuacaoControllerTests
    {
        private readonly HttpClient _cliente;

        public RegraPontuacaoControllerTests(PetBuddiesApiFixture fixture)
        {
            _cliente = fixture.CriarClienteComPerfil("VET");
        }

        // ClinicaId aleatório por teste pelo mesmo motivo do OfertaControllerTests:
        // UK_PONTUACAO_VIGENCIA é por (clínica, gesto, vigência).
        private static SalvarRegraPontuacaoRequest RegraValida() => new()
        {
            ClinicaId = Random.Shared.Next(100_000, 999_999),
            Gesto = TipoGestoEnum.CONSULTA_REALIZADA,
            Pontos = 10,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        [Fact]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var request = RegraValida();

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/regras-pontuacao", request, JsonPadrao.Opcoes);
            var criada = await resposta.Content.ReadFromJsonAsync<RegraPontuacaoDto>(JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            Assert.Equal(request.Pontos, criada!.Pontos);
        }

        [Fact]
        public async Task Cadastrar_PontosNegativos_Retorna400()
        {
            // Arrange — CK_PONTUACAO_PONTOS, provada de ponta a ponta via HTTP.
            var request = RegraValida() with { Pontos = -5 };

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/regras-pontuacao", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_MesmaVigenciaDuasVezes_Retorna409()
        {
            // Arrange — UK_PONTUACAO_VIGENCIA: mesma clínica, gesto e início de vigência.
            var request = RegraValida();
            var primeiroCadastro = await _cliente.PostAsJsonAsync("/api/regras-pontuacao", request, JsonPadrao.Opcoes);
            Assert.Equal(HttpStatusCode.Created, primeiroCadastro.StatusCode);

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/regras-pontuacao", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        }
    }
}
