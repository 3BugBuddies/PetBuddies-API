using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_janela_atendimento")]
    public class JanelaAtendimentoEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("dt_janela")]
        public DateOnly Data { get; set; }

        [Column("hr_inicio")]
        public TimeOnly HoraInicio { get; set; }

        [Column("hr_fim")]
        public TimeOnly HoraFim { get; set; }

        [Column("nr_duracao_slot")]
        [Range(1, 1440)]
        public int DuracaoSlot { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Veterinario))]
        [Column("id_veterinario")]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity? Veterinario { get; set; }
    }
}
