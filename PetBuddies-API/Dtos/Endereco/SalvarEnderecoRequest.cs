using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Endereco
{
    public class SalvarEnderecoRequest
    {
        [Required]
        [StringLength(150)]
        public string Logradouro { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Complemento { get; set; }

        [StringLength(100)]
        public string? Bairro { get; set; }

        [Required]
        [StringLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Estado { get; set; } = string.Empty;

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Cep { get; set; } = string.Empty;
    }
}
