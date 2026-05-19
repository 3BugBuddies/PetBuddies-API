using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_registro_atendimento")]
    public class RegistroAtendimentoEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("dt_atendimento")]
        public DateTime DataAtendimento { get; set; }

        [Column("s_anamnese")]
        [StringLength(2000)]
        public string? Anamnese { get; set; }

        [Column("s_diagnostico")]
        [StringLength(2000)]
        public string? Diagnostico { get; set; }

        [Column("s_tratamento")]
        [StringLength(2000)]
        public string? Tratamento { get; set; }

        [Column("s_observacao")]
        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Column("dt_proximo_retorno")]
        public DateOnly ProximoRetorno { get; set; }

        [Column("dt_proxima_vacina")]
        public DateOnly ProximoVacina { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("id_animal")]
        
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }

        [ForeignKey(nameof(Prontuario))]
        [Column("id_prontuario")]
        public int ProntuarioId { get; set; }

        [JsonIgnore]
        public ProntuarioEntity? Prontuario { get; set; }

        [ForeignKey(nameof(Consulta))]
        [Column("id_consulta")]
        
        public int ConsultaId { get; set; }

        [JsonIgnore]
        public ConsultaEntity? Consulta { get; set; }

        [JsonIgnore]
        public ICollection<ProcedimentoEntity> Procedimentos { get; set; } = [];
    }
}
