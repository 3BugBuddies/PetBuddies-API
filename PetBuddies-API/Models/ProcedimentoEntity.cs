using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_procedimento")]
    public class ProcedimentoEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tp_procedimento")]
        public TipoProcedimentoEnum Tipo { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("s_descricao")]
        [StringLength(2000)]
        public string? Descricao { get; set; }

        [Column("st_procedimento")]
        public StatusProcedimentoEnum Status { get; set; }

        [Column("dt_prevista_inicio")]
        public DateTime DataPrevistaInicio { get; set; }

        [Column("dt_prevista_fim")]
        public DateTime DataPrevistaFim { get; set; }

        [Column("s_anexos_url")]
        [StringLength(500)]
        public string? AnexosUrl { get; set; }

        [Column("s_observacao")]
        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(RegistroAtendimento))]
        [Column("id_registro_atendimento")]
        public int RegistroAtendimentoId { get; set; }

        [JsonIgnore]
        public RegistroAtendimentoEntity? RegistroAtendimento { get; set; }

        [ForeignKey(nameof(Animal))]
        [Column("id_animal")]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public AnimalEntity? Animal { get; set; }
        [ForeignKey(nameof(Veterinario))]
        [Column("id_veterinario")]
        
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity? Veterinario { get; set; }
    }
}
