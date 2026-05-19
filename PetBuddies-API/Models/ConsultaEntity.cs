using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_CONSULTA")]
    public class ConsultaEntity : BaseEntity
    {
        [Key]
        [Column("ID_CONSULTA")]
        public int Id { get; set; }

        [Column("TP_TIPO_CONSULTA")]
        [EnumDataType(typeof(TipoConsultaEnum))]
        public TipoConsultaEnum TipoConsulta { get; set; }

        [Column("DH_DATA_HORA")]
        public DateTime DataHora { get; set; }

        [Column("ST_STATUS_CONSULTA")]
        [EnumDataType(typeof(StatusConsultaEnum))]
        public StatusConsultaEnum Status { get; set; }

        [Column("EM_EMERGENCIA")]
        public bool Emergencia { get; set; }

        [Column("PR_PRIORIDADE")]
        public bool Prioridade { get; set; }

        [Column("OB_OBSERVACAO")]
        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Column("MT_MOTIVO")]
        [StringLength(2000)]
        public string? Motivo { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("ID_ANIMAL")]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }

        [ForeignKey(nameof(Veterinario))]
        [Column("ID_VETERINARIO")]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity? Veterinario { get; set; }

        [ForeignKey(nameof(Clinica))]
        [Column("ID_CLINICA")]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [JsonIgnore]
        public RegistroAtendimentoEntity? RegistroAtendimento { get; set; }
    }
}
