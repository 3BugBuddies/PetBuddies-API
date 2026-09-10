using System.Net;
using System.Net.Http.Json;
using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.Controllers
{
    [Collection(PetBuddiesApiCollection.NomeDaColecao)]
    public class OfertaControllerTests
    {
        private readonly HttpClient _cliente;

        public OfertaControllerTests(PetBuddiesApiFixture fixture)
        {
            _cliente = fixture.CriarClienteComPerfil("VET");
        }

        // ClinicaId aleatório por teste: a base é compartilhada por toda a coleção, e
        // UX_OFERTA_VIGENCIA é por (clínica, ato, subtipo, vigência) — sem isolar por
        // clínica, um teste de conflito interferiria no de sucesso do outro.
        private static SalvarOfertaRequest OfertaValida() => new()
        {
            ClinicaId = Random.Shared.Next(100_000, 999_999),
            Ato = TipoAtoOfertaEnum.CONSULTA,
            Subtipo = "CLINICA_GERAL",
            Descricao = "Consulta de rotina",
            Valor = 120m,
            InicioVigencia = new DateOnly(2026, 1, 1)
        };

        [Fact]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var request = OfertaValida();

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/ofertas", request, JsonPadrao.Opcoes);
            var criada = await resposta.Content.ReadFromJsonAsync<OfertaDto>(JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            Assert.Equal(request.ClinicaId, criada!.ClinicaId);
            Assert.Equal(request.Valor, criada.Valor);
        }

        [Fact]
        public async Task Cadastrar_ValorNegativo_Retorna400()
        {
            // Arrange — CK_OFERTA_VALOR, provada de ponta a ponta via HTTP.
            var request = OfertaValida() with { Valor = -10m };

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/ofertas", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_MesmaVigenciaDuasVezes_Retorna409()
        {
            // Arrange — UX_OFERTA_VIGENCIA: mesma clínica, ato, subtipo e início de vigência.
            var request = OfertaValida();
            var primeiroCadastro = await _cliente.PostAsJsonAsync("/api/ofertas", request, JsonPadrao.Opcoes);
            Assert.Equal(HttpStatusCode.Created, primeiroCadastro.StatusCode);

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/ofertas", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
        }
    }
}
