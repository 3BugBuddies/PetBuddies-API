using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// Cabeçalho de um check-in do tutor: <b>um relato, por pet, num momento</b>.
    ///
    /// <para>Não tem FK para prescrição de propósito. O tutor conta como o pet passou
    /// hoje, e esse mesmo relato é avaliado contra todas as prescrições ativas — uma
    /// linha de <see cref="CheckinResultadoEntity"/> para cada.</para>
    ///
    /// <para>Guarda a <b>transcrição</b>, não o áudio (ADR s3-18).</para>
    /// </summary>
    [Table("T_PB_CHECKIN_TRATAMENTO")]
    [Index(nameof(AnimalId), nameof(RegistradoEm))]
    public class CheckinTratamentoEntity : BaseEntity
    {
        [Key]
        [Column("ID_CHECKIN_TRATAMENTO")]
        public int Id { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("ID_ANIMAL")]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity Animal { get; set; } = null!;

        [Column("DH_REGISTRADO_EM")]
        public DateTime RegistradoEm { get; set; }

        [Required(ErrorMessage = "Narrativa do check-in é obrigatória.")]
        [Column("TX_NARRATIVA", TypeName = "CLOB")]
        public string Narrativa { get; set; } = string.Empty;

        [Column("TX_OBSERVACOES_GERAIS", TypeName = "CLOB")]
        public string? ObservacoesGerais { get; set; }

        /// <summary>Tecnologia usada na coleta do relato — exigência do Art. 13 §5º.</summary>
        [Column("DS_TIC_UTILIZADA")]
        [StringLength(120)]
        public string? TicUtilizada { get; set; }

        [JsonIgnore]
        public ICollection<CheckinExtracaoEntity> Extracoes { get; set; } = [];

        [JsonIgnore]
        public ICollection<CheckinResultadoEntity> Resultados { get; set; } = [];
    }
}
