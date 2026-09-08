using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// Uma condição observada no check-in, e o que fazer com a dose quando ela é
    /// satisfeita. Imutável, como a prescrição que a carrega.
    ///
    /// <para>Guarda uma FK para a condição <b>e</b> uma cópia congelada do rótulo, do
    /// tipo de valor e da fonte. As duas coisas respondem perguntas diferentes: a FK
    /// prova que a condição existe hoje, o congelado prova o que o veterinário
    /// assinou naquele dia. Renomear a condição no catálogo não pode reescrever o
    /// passado (ADR s3-11).</para>
    /// </summary>
    [Table("T_PB_REGRA_PRESCRICAO")]
    public class RegraPrescricaoEntity : BaseEntity
    {
        [Key]
        [Column("ID_REGRA_PRESCRICAO")]
        public int Id { get; set; }

        [ForeignKey(nameof(Prescricao))]
        [Column("ID_PRESCRICAO")]
        public int PrescricaoId { get; set; }

        [JsonIgnore]
        public PrescricaoEntity Prescricao { get; set; } = null!;

        [ForeignKey(nameof(CondicaoClinica))]
        [Column("ID_CONDICAO_CLINICA")]
        public int CondicaoClinicaId { get; set; }

        [JsonIgnore]
        public CondicaoClinicaEntity CondicaoClinica { get; set; } = null!;

        [Required]
        [Column("DS_ROTULO_CONGELADO")]
        [StringLength(255)]
        public string RotuloCongelado { get; set; } = string.Empty;

        [Column("TP_VALOR_CONGELADO")]
        [EnumDataType(typeof(TipoValorCondicaoEnum))]
        public TipoValorCondicaoEnum TipoValorCongelado { get; set; }

        [Column("TP_FONTE_CONGELADA")]
        [EnumDataType(typeof(FonteCondicaoEnum))]
        public FonteCondicaoEnum FonteCongelada { get; set; }

        /// <summary>Obrigatório quando a condição é NUMERICO; nulo quando é BOOLEANO.</summary>
        [Column("TP_OPERADOR")]
        [EnumDataType(typeof(OperadorRegraEnum))]
        public OperadorRegraEnum? Operador { get; set; }

        /// <summary>Obrigatório quando a condição é NUMERICO; nulo quando é BOOLEANO.</summary>
        [Column("NR_LIMITE", TypeName = "NUMBER(10,3)")]
        public decimal? Limite { get; set; }

        [Column("TP_ACAO")]
        [EnumDataType(typeof(TipoAcaoRegraEnum))]
        public TipoAcaoRegraEnum Acao { get; set; }

        /// <summary>Ordem de avaliação. A primeira regra satisfeita decide o desfecho.</summary>
        [Column("NR_ORDEM")]
        [Range(1, 999)]
        public int Ordem { get; set; }
    }
}
