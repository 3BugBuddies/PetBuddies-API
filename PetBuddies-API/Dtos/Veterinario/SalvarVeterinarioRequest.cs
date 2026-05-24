using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Veterinario
{
    public class SalvarVeterinarioRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Crmv { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Especialidade { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        public bool AtendeEmergencia { get; set; }
        public bool Ativo { get; set; } = true;
        [Range(1, int.MaxValue, ErrorMessage = "ClinicaId deve ser maior que zero.")]
        public int ClinicaId { get; set; }
    }
}
