using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PetBuddies_API.Models
{
    [Table("T_PB_JANELA_ATENDIMENTO")]
    [Index(nameof(VeterinarioId), nameof(DataHoraInicio), IsUnique = true)]
    public class JanelaAtendimentoEntity : BaseEntity
    {
        [Key]
        [Column("ID_JANELA_ATENDIMENTO")]
        public int Id { get; set; }

        [Column("DH_DATA_HORA_INICIO")]
        public DateTime DataHoraInicio { get; set; }

        [Column("DH_DATA_HORA_FIM")]
        public DateTime DataHoraFim { get; set; }

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
