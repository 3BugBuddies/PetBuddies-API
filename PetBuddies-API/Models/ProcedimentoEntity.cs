using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("")]
    public class ProcedimentoEntity
    {
        [Key]
        public int Id { get; set; }

        public TipoProcedimentoEnum Tipo { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public StatusProcedimentoEnum Status { get; set; }

        public DateTime DataPrevistaInicio { get; set; }

        public DateTime DataPrecistaFim { get; set; }

        public string AnexosUrl { get; set; }

        public string Observacao { get; set; }

        public DateTime CreatedAt { get; set; }


        [ForeignKey(nameof(RegistroAtendimentoEntity))]
        public int RegistroAtendimentoId { get; set; }

        [JsonIgnore]
        public RegistroAtendimentoEntity? RegistroAtendimento { get; set; }


        [ForeignKey(nameof(AnimalEntity))]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }


        [ForeignKey(nameof(VeterinarioEntity))]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity? Veterinario { get; set; }

    }
}
