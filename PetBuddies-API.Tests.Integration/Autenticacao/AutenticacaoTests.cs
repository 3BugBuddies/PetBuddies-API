using System.Net;
using System.Net.Http.Json;
using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.Autenticacao
{
    // O trio que a rubrica 2.2 pede: sem token, com token do perfil errado, com token do
    // perfil certo. Contra POST /api/protocolos porque o cadastro de protocolo não tem
    // nenhuma regra de unicidade — o 201 do terceiro caso não depende de estado de outro teste.
    [Collection(PetBuddiesApiCollection.NomeDaColecao)]
    public class AutenticacaoTests
    {
        private readonly PetBuddiesApiFixture _fixture;

        public AutenticacaoTests(PetBuddiesApiFixture fixture)
        {
            _fixture = fixture;
        }

        private static SalvarProtocoloRequest ProtocoloValido() => new()
        {
            Nome = $"Protocolo de autenticação {Guid.NewGuid()}",
            Categoria = CategoriaProtocoloEnum.PREVENTIVO,
            Especie = EspecieEnum.CACHORRO,
            Ativo = true
        };

        [Fact]
        public async Task Cadastrar_SemToken_Retorna401()
        {
            // Arrange
            var cliente = _fixture.CreateClient();

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/protocolos", ProtocoloValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_TokenDeTutor_Retorna403()
        {
            // Arrange — token válido, mas o endpoint é [Authorize(Roles = "VET")].
            var cliente = _fixture.CriarClienteComPerfil("TUTOR");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/protocolos", ProtocoloValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
        }

        [Fact]
        public async Task Cadastrar_TokenDeVet_Retorna201()
        {
            // Arrange
            var cliente = _fixture.CriarClienteComPerfil("VET");

            // Act
            var resposta = await cliente.PostAsJsonAsync("/api/protocolos", ProtocoloValido(), JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }
    }
}
