using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_prontuario")]
    public class ProntuarioEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("s_alergias")]
        [StringLength(2000)]
        public string? Alergias { get; set; }

        [Column("s_observacoes")]
        [StringLength(2000)]
        public string? Observacoes { get; set; }

        [Column("dt_atualizado")]
        public DateTime UpdatedAt { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("id_animal")]
        
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }
    }
}
