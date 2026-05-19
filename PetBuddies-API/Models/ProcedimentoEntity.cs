using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_PROCEDIMENTO")]
    public class ProcedimentoEntity : BaseEntity
    {
        [Key]
        [Column("ID_PROCEDIMENTO")]
        public int Id { get; set; }

        [Column("TP_TIPO_PROCEDIMENTO")]
        [EnumDataType(typeof(TipoProcedimentoEnum))]
        public TipoProcedimentoEnum Tipo { get; set; }

        [Required(ErrorMessage = "Nome do procedimento é obrigatório.")]
        [Column("NM_NOME")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do procedimento é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Column("DS_DESCRICAO")]
        [StringLength(2000)]
        public string? Descricao { get; set; }

        [Column("ST_STATUS_PROCEDIMENTO")]
        [EnumDataType(typeof(StatusProcedimentoEnum))]
        public StatusProcedimentoEnum Status { get; set; }

        [Column("DT_DATA_PREVISTA_INICIO")]
        public DateTime DataPrevistaInicio { get; set; }

        [Column("DT_DATA_PREVISTA_FIM")]
        public DateTime DataPrevistaFim { get; set; }

        [Column("AN_ANEXOS_URL")]
        [StringLength(500)]
        public string? AnexosUrl { get; set; }

        [Column("OB_OBSERVACAO")]
        [StringLength(2000)]
        public string? Observacao { get; set; }

        [ForeignKey(nameof(RegistroAtendimento))]
        [Column("ID_REGISTRO_ATENDIMENTO")]
        public int RegistroAtendimentoId { get; set; }

        [JsonIgnore]
        public RegistroAtendimentoEntity? RegistroAtendimento { get; set; }

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
    }
}
