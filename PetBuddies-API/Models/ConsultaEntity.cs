using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_consulta")]
    public class ConsultaEntity
    {
        [Key]
        public int Id { get; set; }

        [Column("")]
        public TipoConsultaEnum TipoConsulta { get; set; }

        public DateTime DataHora { get; set; }

        public StatusConsultaEnum Status {  get; set; }

        public bool Emergencia { get; set; }

        public bool Prioridade { get; set; }

        public string Observacao { get; set; }

        public string Motivo { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }


        [ForeignKey(nameof(AnimalEntity))]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity Animal { get; set; }


        [ForeignKey(nameof(VeterinarioEntity))]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity Veterinario { get; set; }


        [ForeignKey(nameof(ClinicaEntity))]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity Clinica { get; set; }


        public RegistroAtendimentoEntity? RegistroAtendimento { get; set; }


    }
}
