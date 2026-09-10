using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Security;

namespace PetBuddies_API.Tests.Integration.Fixtures
{
    // Uma instância por coleção de teste (Collection Fixture): sobe o host uma única vez
    // e todas as classes de controller a reaproveitam via [Collection(...)].
    public class PetBuddiesApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        // 32+ caracteres — o mínimo que JwtOptions exige. Nunca é a chave real: só existe
        // para o par assinar/validar dentro deste processo de teste.
        private const string SegredoDeTeste = "chave-de-teste-para-jwt-nao-usar-em-producao-9x8";

        private static readonly string NomeDoBancoEmMemoria = $"petbuddies-testes-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, configuracao) =>
            {
                configuracao.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [JwtOptions.VariavelDeAmbienteDoSegredo] = SegredoDeTeste
                });
            });

            builder.ConfigureServices(services =>
            {
                var descritorDoContexto = services.SingleOrDefault(
                    servico => servico.ServiceType == typeof(DbContextOptions<ApplicationContext>));
                if (descritorDoContexto is not null)
                {
                    services.Remove(descritorDoContexto);
                }

                // InMemory no lugar do Oracle: Program.cs já pula o Database.Migrate() em
                // Testing (comentário no próprio arquivo), então só falta trocar o provider.
                services.AddDbContext<ApplicationContext>(options =>
                    options.UseInMemoryDatabase(NomeDoBancoEmMemoria));
            });
        }

        public async Task InitializeAsync()
        {
            using var escopo = Services.CreateScope();
            var contexto = escopo.ServiceProvider.GetRequiredService<ApplicationContext>();
            await contexto.Database.EnsureCreatedAsync();
        }

        public new Task DisposeAsync() => Task.CompletedTask;

        public HttpClient CriarClienteComPerfil(string perfil)
        {
            var cliente = CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CriarToken(perfil));
            return cliente;
        }

        // Mesma chave e mesmo emissor que o Program.cs valida — gerado com os JwtOptions
        // resolvidos do próprio host de teste, não hardcoded, para nunca dessincronizar.
        public string CriarToken(string perfil)
        {
            var jwtOptions = Services.GetRequiredService<IOptions<JwtOptions>>().Value;

            var credenciais = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                claims: new[] { new Claim("perfil", perfil) },
                notBefore: DateTime.UtcNow.AddMinutes(-1),
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credenciais);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    [CollectionDefinition(NomeDaColecao)]
    public class PetBuddiesApiCollection : ICollectionFixture<PetBuddiesApiFixture>
    {
        public const string NomeDaColecao = "API PetBuddies (integração)";
    }
}
