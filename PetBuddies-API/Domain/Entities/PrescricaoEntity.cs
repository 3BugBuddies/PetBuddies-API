using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    /// <summary>
    /// Ato assinado pelo veterinário no fechamento do atendimento: o que o animal
    /// toma, em que faixa de dose, com que frequência e por quantos dias.
    ///
    /// É <b>imutável</b> por contrato (ADR s3-09) — <see cref="BaseEntity.UpdatedAt"/>
    /// fica sempre nulo. Corrigir uma prescrição significa emitir outra, não editar
    /// esta: o que foi assinado precisa continuar legível depois.
    /// </summary>
    [Table("T_PB_PRESCRICAO")]
    public class PrescricaoEntity : BaseEntity
    {
        [Key]
        [Column("ID_PRESCRICAO")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do medicamento é obrigatório.")]
        [Column("NM_MEDICAMENTO")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do medicamento é obrigatório.")]
        public string Medicamento { get; set; } = string.Empty;

        [Column("NR_DOSE_MIN", TypeName = "NUMBER(8,3)")]
        public decimal DoseMin { get; set; }

        [Column("NR_DOSE_MAX", TypeName = "NUMBER(8,3)")]
        public decimal DoseMax { get; set; }

        [Required(ErrorMessage = "Unidade da dose é obrigatória.")]
        [Column("DS_UNIDADE")]
        [StringLength(20)]
        public string Unidade { get; set; } = string.Empty;

        [Column("NR_FREQUENCIA_DIA")]
        [Range(1, 99)]
        public int FrequenciaDia { get; set; }

        [Column("NR_DURACAO_DIAS")]
        [Range(1, 9999)]
        public int DuracaoDias { get; set; }

        [Column("DT_INICIO")]
        public DateOnly DataInicio { get; set; }

        [Column("TX_ORIENTACAO", TypeName = "CLOB")]
        public string? Orientacao { get; set; }

        /// <summary>Material do bulário que originou a prescrição, quando houve um (ADR s3-10).</summary>
        [Column("ID_MATERIAL_ORIGEM")]
        public int? MaterialOrigemId { get; set; }

        [Column("NR_VERSAO_ORIGEM")]
        public int? VersaoOrigem { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("ID_ANIMAL")]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity Animal { get; set; } = null!;

        [ForeignKey(nameof(Veterinario))]
        [Column("ID_VETERINARIO")]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity Veterinario { get; set; } = null!;

        [ForeignKey(nameof(RegistroAtendimento))]
        [Column("ID_REGISTRO_ATENDIMENTO")]
        public int RegistroAtendimentoId { get; set; }

        [JsonIgnore]
        public RegistroAtendimentoEntity RegistroAtendimento { get; set; } = null!;

        [JsonIgnore]
        public ICollection<RegraPrescricaoEntity> Regras { get; set; } = [];
    }
}
