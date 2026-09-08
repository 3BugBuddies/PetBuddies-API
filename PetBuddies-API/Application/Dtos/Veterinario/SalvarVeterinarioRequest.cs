using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.Veterinario
{
    public class SalvarVeterinarioRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Crmv { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        public bool Ativo { get; set; } = true;
        [Range(1, int.MaxValue, ErrorMessage = "ClinicaId deve ser maior que zero.")]
        public int ClinicaId { get; set; }
    }
}
