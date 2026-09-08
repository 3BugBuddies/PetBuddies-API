using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    /// <summary>
    /// Uma condição que a IA reconheceu na narrativa do tutor, com o valor observado
    /// e a confiança da extração. Imutável.
    ///
    /// <para>Pendura no cabeçalho do check-in, não na prescrição: "as fezes estavam
    /// moles" é observação sobre o pet e vale para todas as prescrições avaliadas
    /// naquele momento.</para>
    ///
    /// <para><b>Exatamente um</b> dos dois campos de valor é preenchido, conforme o
    /// tipo da condição. O banco garante por CHECK; a validação de shape que espelha
    /// isso no código entra nos PRs de produto.</para>
    /// </summary>
    [Table("T_PB_CHECKIN_EXTRACAO")]
    public class CheckinExtracaoEntity : BaseEntity
    {
        [Key]
        [Column("ID_CHECKIN_EXTRACAO")]
        public int Id { get; set; }

        [ForeignKey(nameof(CheckinTratamento))]
        [Column("ID_CHECKIN_TRATAMENTO")]
        public int CheckinTratamentoId { get; set; }

        [JsonIgnore]
        public CheckinTratamentoEntity CheckinTratamento { get; set; } = null!;

        [ForeignKey(nameof(CondicaoClinica))]
        [Column("ID_CONDICAO_CLINICA")]
        public int CondicaoClinicaId { get; set; }

        [JsonIgnore]
        public CondicaoClinicaEntity CondicaoClinica { get; set; } = null!;

        [Required]
        [Column("CD_CODIGO_CONGELADO")]
        [StringLength(60)]
        public string CodigoCongelado { get; set; } = string.Empty;

        [Column("BL_VALOR_BOOLEANO")]
        public bool? ValorBooleano { get; set; }

        [Column("NR_VALOR_NUMERICO", TypeName = "NUMBER(10,3)")]
        public decimal? ValorNumerico { get; set; }

        /// <summary>Confiança da extração, entre 0 e 1. É probabilidade, não escala livre.</summary>
        [Column("NR_CONFIANCA", TypeName = "NUMBER(5,4)")]
        [Range(0, 1)]
        public decimal Confianca { get; set; }
    }
}
