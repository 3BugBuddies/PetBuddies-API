using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_consulta")]
    public class ConsultaEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tp_consulta")]
        public TipoConsultaEnum TipoConsulta { get; set; }

        [Column("dt_consulta")]
        public DateTime DataHora { get; set; }

        [Column("st_consulta")]
        public StatusConsultaEnum Status { get; set; }

        [Column("bl_emergencia")]
        public bool Emergencia { get; set; }

        [Column("bl_prioridade")]
        public bool Prioridade { get; set; }

        [Column("s_observacao")]
        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Column("s_motivo")]
        [StringLength(2000)]
        public string? Motivo { get; set; }

        [Column("dt_atualizado")]
        public DateTime UpdatedAt { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

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

        [ForeignKey(nameof(Clinica))]
        [Column("id_clinica")]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [JsonIgnore]
        public RegistroAtendimentoEntity? RegistroAtendimento { get; set; }
    }
}
