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

        [ForeignKey(nameof(Veterinario))]
        [Column("ID_VETERINARIO")]
        public int VeterinarioId { get; set; }

        [JsonIgnore]
        public VeterinarioEntity? Veterinario { get; set; }

        /// <summary>
        /// A consulta que reservou este slot, ou nulo se o horário está livre.
        ///
        /// <para>A reserva passou a ser referência persistida, e não comparação de
        /// timestamp. Isso deixa uma consulta longa ocupar slots consecutivos — mas
        /// <b>obriga o cancelamento a apagar o vínculo</b>. Esquecer não dá erro: o
        /// slot fica ocupado para sempre e o sintoma aparece depois como "a agenda não
        /// abre horário". <c>OnDelete SetNull</c> cobre exclusão, não cobre
        /// cancelamento — são caminhos diferentes. O tratamento é do PR N8.</para>
        /// </summary>
        [ForeignKey(nameof(Consulta))]
        [Column("ID_CONSULTA")]
        public int? ConsultaId { get; set; }

        [JsonIgnore]
        public ConsultaEntity? Consulta { get; set; }
    }
}
