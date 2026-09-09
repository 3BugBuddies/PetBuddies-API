using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Infrastructure.Security
{
    /// <summary>
    /// O formato do token que o Java emite (ADR s3-20). Este serviço só valida.
    /// </summary>
    /// <remarks>
    /// <b>O segredo nunca vem de <c>appsettings</c> versionado.</b> Ele é lido da variável de
    /// ambiente <c>PETBUDDIES_JWT_SECRET</c>, com o mesmo valor nos dois serviços — credencial
    /// no fonte é penalizada pela frente de DevOps.
    /// </remarks>
    public class JwtOptions
    {
        public const string SecaoConfiguracao = "Jwt";

        /// <summary>Nome da variável de ambiente que carrega a chave simétrica HS256.</summary>
        public const string VariavelDeAmbienteDoSegredo = "PETBUDDIES_JWT_SECRET";

        /// <summary>Mínimo de 32 bytes, exigência do HS256 (ADR s3-20).</summary>
        [Required(ErrorMessage = "Defina a variável de ambiente PETBUDDIES_JWT_SECRET.")]
        [MinLength(32, ErrorMessage = "PETBUDDIES_JWT_SECRET precisa de pelo menos 32 caracteres.")]
        public string Secret { get; set; } = string.Empty;

        /// <summary>Quem emite o token. É o serviço Java, e não este.</summary>
        [Required]
        public string Issuer { get; set; } = "petbuddies-ai";
    }
}
