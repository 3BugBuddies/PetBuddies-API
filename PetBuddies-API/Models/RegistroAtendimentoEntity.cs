using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_REGISTRO_ATENDIMENTO")]
    public class RegistroAtendimentoEntity : BaseEntity
    {
        [Key]
        [Column("ID_REGISTRO_ATENDIMENTO")]
        public int Id { get; set; }

        [Column("DT_DATA_ATENDIMENTO")]
        public DateTime DataAtendimento { get; set; }

        [Column("AN_ANAMNESE")]
        [StringLength(2000)]
        public string? Anamnese { get; set; }

        [Column("DG_DIAGNOSTICO")]
        [StringLength(2000)]
        public string? Diagnostico { get; set; }

        [Column("TR_TRATAMENTO")]
        [StringLength(2000)]
        public string? Tratamento { get; set; }

        [Column("OB_OBSERVACAO")]
        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Column("PR_PROXIMO_RETORNO")]
        public DateOnly? ProximoRetorno { get; set; }

        [Column("PR_PROXIMA_VACINA")]
        public DateOnly? ProximaVacina { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("ID_ANIMAL")]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }

        [ForeignKey(nameof(Prontuario))]
        [Column("ID_PRONTUARIO")]
        public int ProntuarioId { get; set; }

        [JsonIgnore]
        public ProntuarioEntity? Prontuario { get; set; }

        [ForeignKey(nameof(Consulta))]
        [Column("ID_CONSULTA")]
        public int ConsultaId { get; set; }

        [JsonIgnore]
        public ConsultaEntity? Consulta { get; set; }

        [JsonIgnore]
        public ICollection<ProcedimentoEntity> Procedimentos { get; set; } = [];
    }
}
