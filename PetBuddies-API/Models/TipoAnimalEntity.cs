using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_TIPO_ANIMAL")]
    public class TipoAnimalEntity : BaseEntity
    {
        [Key]
        [Column("ID_TIPO_ANIMAL")]
        public int Id { get; set; }

        [Column("ES_ESPECIE")]
        [EnumDataType(typeof(EspecieEnum))]
        public EspecieEnum Especie { get; set; }

        [Column("RC_RACA")]
        [StringLength(100)]
        public string Raca { get; set; } = "SEM_RACA";

        [Column("PT_PORTE")]
        [EnumDataType(typeof(PorteEnum))]
        public PorteEnum Porte { get; set; }

        [JsonIgnore]
        public ICollection<AnimalEntity> Animais { get; set; } = [];
    }
}
