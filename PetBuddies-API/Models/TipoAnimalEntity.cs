using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_tipo_animal")]
    public class TipoAnimalEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tp_especie")]
        public EspecieEnum Especie { get; set; }

        [Column("s_raca")]
        [StringLength(100)]
        public string Raca { get; set; } = string.Empty;

        [Column("tp_porte")]
        public PorteEnum Porte { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<AnimalEntity> Animais { get; set; } = [];
    }
}
