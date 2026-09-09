using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Infrastructure.Security
{
    public class JwtOptions
    {
        public const string SecaoConfiguracao = "Jwt";

        public const string VariavelDeAmbienteDoSegredo = "PETBUDDIES_JWT_SECRET";

        // 32 caracteres é o mínimo da chave simétrica do HS256.
        [Required(ErrorMessage = "Defina a variável de ambiente PETBUDDIES_JWT_SECRET.")]
        [MinLength(32, ErrorMessage = "PETBUDDIES_JWT_SECRET precisa de pelo menos 32 caracteres.")]
        public string Secret { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = "petbuddies-ai";
    }
}
