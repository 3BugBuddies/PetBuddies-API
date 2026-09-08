using Microsoft.EntityFrameworkCore;
using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// O desfecho de uma prescrição num check-in: a dose do dia, a orientação de
    /// procurar a clínica, ou nenhuma dose. Uma linha por prescrição avaliada.
    ///
    /// <para>O índice único <c>(PrescricaoId, DataReferencia)</c> é <b>a trava da
    /// adesão</b>: sem ele o mesmo dia de tratamento entra duas vezes e "dia 4 de 10"
    /// passa a contar 11. Ele só funciona de verdade quando
    /// <see cref="DataReferencia"/> for gravada como DATE, e não como texto — é o que
    /// o PR seguinte (N2) corrige.</para>
    /// </summary>
    [Table("T_PB_CHECKIN_RESULTADO")]
    [Index(nameof(PrescricaoId), nameof(DataReferencia), IsUnique = true)]
    public class CheckinResultadoEntity : BaseEntity
    {
        [Key]
        [Column("ID_CHECKIN_RESULTADO")]
        public int Id { get; set; }

        [ForeignKey(nameof(CheckinTratamento))]
        [Column("ID_CHECKIN_TRATAMENTO")]
        public int CheckinTratamentoId { get; set; }

        [JsonIgnore]
        public CheckinTratamentoEntity CheckinTratamento { get; set; } = null!;

        [ForeignKey(nameof(Prescricao))]
        [Column("ID_PRESCRICAO")]
        public int PrescricaoId { get; set; }

        [JsonIgnore]
        public PrescricaoEntity Prescricao { get; set; } = null!;

        /// <summary>O dia de tratamento a que este desfecho se refere.</summary>
        [Column("DT_REFERENCIA")]
        public DateOnly DataReferencia { get; set; }

        [Column("TP_DESFECHO")]
        [EnumDataType(typeof(DesfechoCheckinEnum))]
        public DesfechoCheckinEnum Desfecho { get; set; }

        /// <summary>Preenchida apenas quando o desfecho é DOSE_CALCULADA.</summary>
        [Column("NR_DOSE_APLICADA", TypeName = "NUMBER(8,3)")]
        public decimal? DoseAplicada { get; set; }

        /// <summary>A regra que decidiu o desfecho, quando alguma decidiu.</summary>
        [ForeignKey(nameof(RegraAplicada))]
        [Column("ID_REGRA_APLICADA")]
        public int? RegraAplicadaId { get; set; }

        [JsonIgnore]
        public RegraPrescricaoEntity? RegraAplicada { get; set; }
    }
}
