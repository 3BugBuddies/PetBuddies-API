using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_tipoAnimal")]
    public class TipoAnimalEntity
    {
        [Key]
        public int Id { get; set; }

        public EspecieEnum Especie { get; set; }

        public string Raca { get; set; }

        public PorteEnum Porte { get; set; }

        public DateTime CreatedAt { get; set; }
        

        [ForeignKey(nameof(AnimalEntity))]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }
    }
}
