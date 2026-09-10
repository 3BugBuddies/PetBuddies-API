using System.Net;
using System.Net.Http.Json;
using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.Controllers
{
    [Collection(PetBuddiesApiCollection.NomeDaColecao)]
    public class RegraProtocoloControllerTests
    {
        private readonly HttpClient _cliente;

        public RegraProtocoloControllerTests(PetBuddiesApiFixture fixture)
        {
            _cliente = fixture.CriarClienteComPerfil("VET");
        }

        private async Task<long> CriarProtocoloAsync()
        {
            var request = new SalvarProtocoloRequest
            {
                Nome = $"Protocolo para regra {Guid.NewGuid()}",
                Categoria = CategoriaProtocoloEnum.PREVENTIVO,
                Especie = EspecieEnum.CACHORRO,
                Ativo = true
            };

            var resposta = await _cliente.PostAsJsonAsync("/api/protocolos", request, JsonPadrao.Opcoes);
            var criado = await resposta.Content.ReadFromJsonAsync<ProtocoloDto>(JsonPadrao.Opcoes);
            return criado!.Id;
        }

        private static SalvarRegraProtocoloRequest RegraValida(long protocoloId) => new()
        {
            ProtocoloId = protocoloId,
            Tipo = TipoCuidadoEnum.VACINACAO,
            Nome = "Primeira dose",
            Offset = 0,
            UnidadeOffset = UnidadeTempoEnum.DIAS,
            DataBase = TipoDataBaseEnum.NASCIMENTO,
            Repeticoes = 1
        };

        [Fact]
        public async Task Cadastrar_RequestValido_Retorna201()
        {
            // Arrange
            var protocoloId = await CriarProtocoloAsync();
            var request = RegraValida(protocoloId);

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/regras-protocolo", request, JsonPadrao.Opcoes);
            var criada = await resposta.Content.ReadFromJsonAsync<RegraProtocoloDto>(JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            Assert.Equal(protocoloId, criada!.ProtocoloId);
        }

        [Fact]
        public async Task Cadastrar_ProtocoloInexistente_Retorna404()
        {
            // Arrange
            var request = RegraValida(long.MaxValue);

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/regras-protocolo", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_IntervaloSemUnidade_Retorna400()
        {
            // Arrange — CK_REGPROT_RECORRENCIA, provada de ponta a ponta via HTTP.
            var protocoloId = await CriarProtocoloAsync();
            var request = RegraValida(protocoloId) with { Intervalo = 30, UnidadeIntervalo = null };

            // Act
            var resposta = await _cliente.PostAsJsonAsync("/api/regras-protocolo", request, JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }
    }
}
