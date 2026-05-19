using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_veterinario")]
    public class VeterinarioEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("s_crmv")]
        [StringLength(30)]
        public string Crmv { get; set; } = string.Empty;

        [Column("s_especialidade")]
        [StringLength(100)]
        public string? Especialidade { get; set; }

        [Column("s_telefone")]
        [StringLength(20)]
        public string? Telefone { get; set; }

        [Column("s_email")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [Column("bl_atende_emergencia")]
        public bool AtendeEmergencia { get; set; }

        [Column("bl_ativo")]
        public bool Ativo { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Clinica))]
        [Column("id_clinica")]
        
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [JsonIgnore]
        public ICollection<ConsultaEntity> Consultas { get; set; } = [];

        [JsonIgnore]
        public ICollection<ProcedimentoEntity> Procedimentos { get; set; } = [];

        [JsonIgnore]
        public ICollection<JanelaAtendimentoEntity> JanelasAtendimento { get; set; } = [];
    }
}
