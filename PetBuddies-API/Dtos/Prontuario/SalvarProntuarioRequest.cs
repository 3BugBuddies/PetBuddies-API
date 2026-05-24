using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Prontuario
{
    public class SalvarProntuarioRequest
    {
        [StringLength(2000)]
        public string? Alergias { get; set; }

        [StringLength(2000)]
        public string? Observacoes { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AnimalId deve ser maior que zero.")]
        public int AnimalId { get; set; }
    }
}
