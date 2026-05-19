using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("")]
    public class RegistroAtendimentoEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime DataAtendimento { get; set; }

        public string Anamnese { get; set; }

        public string Diagnostico { get; set; }

        public string Tratamento { get; set; }

        public string Observacao { get; set; }

        public DateOnly ProximoRetorno { get; set; }

        public DateOnly ProximoVacina { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(AnimalEntity))]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }


        [ForeignKey(nameof(ProntuarioEntity))]
        public int ProntuarioId { get; set; }

        [JsonIgnore]
        public ProntuarioEntity? Prontuario { get; set; }


        [ForeignKey(nameof(ConsultaEntity))]
        public int ConsultaId { get; set; }

        [JsonIgnore]
        public ConsultaEntity? Consulta { get; set; }


        public ICollection<ProcedimentoEntity> Procedimentos { get; set; }
    }
}
