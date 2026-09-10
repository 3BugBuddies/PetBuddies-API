using System.Net;
using System.Net.Http.Json;
using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Tests.Integration.Fixtures;

namespace PetBuddies_API.Tests.Integration.Controllers
{
    [Collection(PetBuddiesApiCollection.NomeDaColecao)]
    public class ProtocoloControllerTests
    {
        private readonly HttpClient _cliente;

        public ProtocoloControllerTests(PetBuddiesApiFixture fixture)
        {
            _cliente = fixture.CriarClienteComPerfil("VET");
        }

        private static SalvarProtocoloRequest ProtocoloValido() => new()
        {
            Nome = $"Protocolo {Guid.NewGuid()}",
            Categoria = CategoriaProtocoloEnum.PREVENTIVO,
            Especie = EspecieEnum.GATO,
            Ativo = true,
            Descricao = "Criado pelo teste de integração"
        };

        [Fact]
        public async Task BuscarPorId_ProtocoloExistente_Retorna200ComOsDados()
        {
            // Arrange
            var request = ProtocoloValido();
            var cadastro = await _cliente.PostAsJsonAsync("/api/protocolos", request, JsonPadrao.Opcoes);
            var criado = await cadastro.Content.ReadFromJsonAsync<ProtocoloDto>(JsonPadrao.Opcoes);

            // Act
            var resposta = await _cliente.GetAsync($"/api/protocolos/{criado!.Id}");
            var encontrado = await resposta.Content.ReadFromJsonAsync<ProtocoloDto>(JsonPadrao.Opcoes);

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(request.Nome, encontrado!.Nome);
            Assert.Equal(request.Categoria, encontrado.Categoria);
            Assert.Equal(request.Especie, encontrado.Especie);
        }

        [Fact]
        public async Task BuscarPorId_IdInexistente_Retorna404()
        {
            // Act
            var resposta = await _cliente.GetAsync($"/api/protocolos/{long.MaxValue}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Listar_FiltradoPorAtivoFalso_Retorna204SemConteudo()
        {
            // Arrange — nenhum teste deste projeto cadastra protocolo inativo, então o
            // filtro abaixo é garantidamente vazio, não importa a ordem de execução.

            // Act
            var resposta = await _cliente.GetAsync("/api/protocolos?ativo=false");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }
    }
}
