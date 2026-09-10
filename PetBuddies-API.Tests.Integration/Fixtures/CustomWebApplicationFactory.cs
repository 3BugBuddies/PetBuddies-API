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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Infrastructure.Data;
using PetBuddies_API.Infrastructure.Security;

namespace PetBuddies_API.Tests.Integration.Fixtures
{
    // Mesma base do PetBuddiesApiFixture (InMemory + segredo do JWT) e, além dela, troca
    // cada Service por um mock singleton — isola o Controller do resto do pipeline, no
    // padrão do professor (RemoveAll + AddSingleton dentro de ConfigureServices).
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private const string SegredoDeTeste = "chave-de-teste-para-jwt-nao-usar-em-producao-9x8";

        // Campo de instância, não estático: cada classe de teste recebe sua própria
        // factory via IClassFixture, então cada uma já nasce com banco próprio.
        private readonly string _nomeDoBancoEmMemoria = $"petbuddies-controller-{Guid.NewGuid()}";

        public Mock<IProtocoloService> ProtocoloServiceMock { get; } = new();
        public Mock<IRegraProtocoloService> RegraProtocoloServiceMock { get; } = new();
        public Mock<IOfertaService> OfertaServiceMock { get; } = new();
        public Mock<IRegraPontuacaoService> RegraPontuacaoServiceMock { get; } = new();

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

                services.AddDbContext<ApplicationContext>(options =>
                    options.UseInMemoryDatabase(_nomeDoBancoEmMemoria));

                services.RemoveAll(typeof(IProtocoloService));
                services.AddSingleton(ProtocoloServiceMock.Object);

                services.RemoveAll(typeof(IRegraProtocoloService));
                services.AddSingleton(RegraProtocoloServiceMock.Object);

                services.RemoveAll(typeof(IOfertaService));
                services.AddSingleton(OfertaServiceMock.Object);

                services.RemoveAll(typeof(IRegraPontuacaoService));
                services.AddSingleton(RegraPontuacaoServiceMock.Object);
            });
        }

        public HttpClient CriarClienteComPerfil(string perfil)
        {
            var cliente = CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CriarToken(perfil));
            return cliente;
        }

        private string CriarToken(string perfil)
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
}
