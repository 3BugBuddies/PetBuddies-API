using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.TipoAnimal
{
    public class TipoAnimalDto
    {
        public int Id { get; set; }
        public EspecieEnum Especie { get; set; }
        public string Raca { get; set; } = string.Empty;
        public PorteEnum Porte { get; set; }
    }
}
