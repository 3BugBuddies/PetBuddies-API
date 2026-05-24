using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Clinica
{
    public class SalvarClinicaRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(14, MinimumLength = 14)]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "EnderecoId deve ser maior que zero.")]
        public int EnderecoId { get; set; }
    }
}
