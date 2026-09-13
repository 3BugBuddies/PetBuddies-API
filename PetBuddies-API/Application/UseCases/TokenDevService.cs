using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetBuddies_API.Application.Dtos.Dev;
using PetBuddies_API.Application.Interfaces;
using PetBuddies_API.Domain.Enums;
using PetBuddies_API.Infrastructure.Security;

namespace PetBuddies_API.Application.UseCases
{
    public class TokenDevService : ITokenDevService
    {
        private const int HorasDeValidade = 8;

        private readonly JwtOptions _opcoes;

        public TokenDevService(IOptions<JwtOptions> opcoes)
        {
            _opcoes = opcoes.Value;
        }

        public TokenDevDto Emitir(PerfilEnum perfil)
        {
            var credenciais = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.Secret)),
                SecurityAlgorithms.HmacSha256);

            // As claims sao as mesmas que o Java emite, e o emissor tambem: um token
            // daqui e indistinguivel de um de la para quem valida.
            var token = new JwtSecurityToken(
                issuer: _opcoes.Issuer,
                claims: new[] { new Claim("perfil", perfil.ToString()) },
                notBefore: DateTime.UtcNow.AddMinutes(-1),
                expires: DateTime.UtcNow.AddHours(HorasDeValidade),
                signingCredentials: credenciais);

            return new TokenDevDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Perfil = perfil,
                Emissor = _opcoes.Issuer,
                ExpiraEm = token.ValidTo
            };
        }
    }
}
