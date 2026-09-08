using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    /// <summary>
    /// Vocabulário clínico de uma clínica: o conjunto de condições que uma regra de
    /// prescrição pode observar. Não há catálogo global — cada clínica provisiona o
    /// seu, e toda condição é assinada por um veterinário.
    /// </summary>
    [Table("T_PB_CONDICAO_CLINICA")]
    [Index(nameof(ClinicaId), nameof(Codigo), IsUnique = true)]
    public class CondicaoClinicaEntity : BaseEntity
    {
        [Key]
        [Column("ID_CONDICAO_CLINICA")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Código da condição é obrigatório.")]
        [Column("CD_CODIGO")]
        [StringLength(60)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Código da condição é obrigatório.")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rótulo da condição é obrigatório.")]
        [Column("DS_ROTULO")]
        [StringLength(255)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Rótulo da condição é obrigatório.")]
        public string Rotulo { get; set; } = string.Empty;

        [Column("TP_VALOR")]
        [EnumDataType(typeof(TipoValorCondicaoEnum))]
        public TipoValorCondicaoEnum TipoValor { get; set; }

        [Column("TP_FONTE")]
        [EnumDataType(typeof(FonteCondicaoEnum))]
        public FonteCondicaoEnum Fonte { get; set; } = FonteCondicaoEnum.RELATO;

        [Column("DS_UNIDADE")]
        [StringLength(20)]
        public string? Unidade { get; set; }

        /// <summary>Marca a condição como crítica: satisfazê-la escala o caso (ADR s3-16).</summary>
        [Column("FL_CRITICA")]
        public bool Critica { get; set; }

        [Column("AT_ATIVO")]
        public bool Ativo { get; set; } = true;

        [ForeignKey(nameof(Clinica))]
        [Column("ID_CLINICA")]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity Clinica { get; set; } = null!;

        /// <summary>Quem assinou a condição. Obrigatório em toda condição, não só nas críticas.</summary>
        [ForeignKey(nameof(VeterinarioAutor))]
        [Column("ID_VETERINARIO_AUTOR")]
        public int VeterinarioAutorId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity VeterinarioAutor { get; set; } = null!;
    }
}
