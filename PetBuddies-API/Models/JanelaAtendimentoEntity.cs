using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("")]
    public class JanelaAtendimentoEntity
    {
        [Key]
        public int Id { get; set; }

        public DateOnly Data { get; set; }

        public TimeOnly HoraInicio { get; set; }

        public TimeOnly HoraFim { get; set; }

        public int DuracaoSlot { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(VeterinarioEntity))]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity Veterinario { get; set; }

    }
}
