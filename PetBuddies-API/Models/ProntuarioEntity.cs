using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_PRONTUARIO")]
    public class ProntuarioEntity : BaseEntity
    {
        [Key]
        [Column("ID_PRONTUARIO")]
        public int Id { get; set; }

        [Column("OB_ALERGIA")]
        [StringLength(2000)]
        public string? Alergias { get; set; }

        [Column("OB_OBSERVACOES")]
        [StringLength(2000)]
        public string? Observacoes { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("ID_ANIMAL")]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }
    }
}
