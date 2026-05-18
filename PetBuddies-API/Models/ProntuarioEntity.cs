using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    public class ProntuarioEntity
    {
        [Key]
        public int Id { get; set; }

        public string Alergias { get; set; }

        public string Observacoes { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }


        [ForeignKey(nameof(AnimalEntity))]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }


    }
}
