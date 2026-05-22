using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Prontuario
{
    public class SalvarProntuarioRequest
    {
        [StringLength(2000)]
        public string? Alergias { get; set; }

        [StringLength(2000)]
        public string? Observacoes { get; set; }

        public int AnimalId { get; set; }
    }
}
