using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// O pet. Absorveu prontuário e tipo de animal na Sprint 3 (decisão H).
    ///
    /// <para>A fusão do tipo conserta um defeito, além de tirar uma tabela:
    /// <c>TipoAnimalEntity</c> guardava o porte com uma coleção de animais atrás, então
    /// corrigir o porte de um pet alterava o de todos que compartilhavam a linha.
    /// Porte é do indivíduo, não da raça.</para>
    /// </summary>
    [Table("T_PB_ANIMAL")]
    public class AnimalEntity : BaseEntity
    {
        [Key]
        [Column("ID_ANIMAL")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do animal é obrigatório.")]
        [Column("NM_NOME_ANIMAL")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do animal é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Column("ES_ESPECIE")]
        [EnumDataType(typeof(EspecieEnum))]
        public EspecieEnum Especie { get; set; }

        [Column("RC_RACA")]
        [StringLength(100)]
        public string Raca { get; set; } = "SEM_RACA";

        [Column("PT_PORTE")]
        [EnumDataType(typeof(PorteEnum))]
        public PorteEnum Porte { get; set; }

        [Column("SX_SEXO")]
        [EnumDataType(typeof(SexoEnum))]
        public SexoEnum Sexo { get; set; }

        [Column("DT_DATA_NASCIMENTO")]
        public DateOnly DataNascimento { get; set; }

        [Column("NR_PESO", TypeName = "NUMBER(5,2)")]
        [Range(0, 999.99)]
        public decimal? Peso { get; set; }

        [Column("CN_CONDICAO_CRONICA")]
        public bool CondicaoCronica { get; set; }

        [Column("CT_CASTRADO")]
        public bool Castrado { get; set; }

        [Column("FT_FOTO")]
        [StringLength(500)]
        public string? Foto { get; set; }

        [Column("OB_ALERGIA")]
        [StringLength(2000)]
        public string? Alergias { get; set; }

        [Column("OB_OBSERVACOES")]
        [StringLength(2000)]
        public string? Observacoes { get; set; }

        [ForeignKey(nameof(Responsavel))]
        [Column("ID_RESPONSAVEL")]
        public int ResponsavelId { get; set; }

        [JsonIgnore]
        public ResponsavelEntity? Responsavel { get; set; }

        [JsonIgnore]
        public ICollection<RegistroAtendimentoEntity> RegistroAtendimentos { get; set; } = [];

        [JsonIgnore]
        public ICollection<ConsultaEntity> Consultas { get; set; } = [];

        [JsonIgnore]
        public ICollection<ProcedimentoEntity> Procedimentos { get; set; } = [];

        [JsonIgnore]
        public ICollection<PrescricaoEntity> Prescricoes { get; set; } = [];

        [JsonIgnore]
        public ICollection<CheckinTratamentoEntity> Checkins { get; set; } = [];
    }
}
