using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_JANELA_ATENDIMENTO")]
    public class JanelaAtendimentoEntity : BaseEntity
    {
        [Key]
        [Column("ID_JANELA_ATENDIMENTO")]
        public int Id { get; set; }

        [Column("CA_DATA")]
        public DateOnly Data { get; set; }

        [Column("HR_HORA_INICIO")]
        public TimeOnly HoraInicio { get; set; }

        [Column("HR_HORA_FIM")]
        public TimeOnly HoraFim { get; set; }

        [Column("DR_DURACAO_SLOT")]
        [Range(1, 1440)]
        public int DuracaoSlot { get; set; }

        [ForeignKey(nameof(Veterinario))]
        [Column("ID_VETERINARIO")]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity? Veterinario { get; set; }
    }
}
