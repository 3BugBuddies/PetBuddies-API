using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.TipoAnimal
{
    public class SalvarTipoAnimalRequest
    {
        [Required]
        public EspecieEnum? Especie { get; set; }

        [StringLength(100)]
        public string Raca { get; set; } = string.Empty;

        [Required]
        public PorteEnum? Porte { get; set; }
    }
}
