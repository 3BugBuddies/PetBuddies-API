using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
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

        [Column("PC_PRE_CADASTRO")]
        public bool PreCadastro { get; set; }

        [Column("CT_CASTRADO")]
        public bool Castrado { get; set; }

        [Column("FT_FOTO")]
        [StringLength(500)]
        public string? Foto { get; set; }

        [ForeignKey(nameof(Responsavel))]
        [Column("ID_RESPONSAVEL")]
        public int ResponsavelId { get; set; }

        [JsonIgnore]
        public ResponsavelEntity? Responsavel { get; set; }

        [ForeignKey(nameof(TipoAnimal))]
        [Column("ID_TIPO_ANIMAL")]
        public int TipoAnimalId { get; set; }

        [JsonIgnore]
        public TipoAnimalEntity? TipoAnimal { get; set; }

        [JsonIgnore]
        public ProntuarioEntity? Prontuario { get; set; }

        [JsonIgnore]
        public ICollection<RegistroAtendimentoEntity> RegistroAtendimentos { get; set; } = [];

        [JsonIgnore]
        public ICollection<ConsultaEntity> Consultas { get; set; } = [];

        [JsonIgnore]
        public ICollection<ProcedimentoEntity> Procedimentos { get; set; } = [];
    }
}
