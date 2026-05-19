using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_VETERINARIO")]
    public class VeterinarioEntity : BaseEntity
    {
        [Key]
        [Column("ID_VETERINARIO")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do veterinário é obrigatório.")]
        [Column("NM_NOME_VETERINARIO")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do veterinário é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CRMV do veterinário é obrigatório.")]
        [Column("NR_CRMV")]
        [StringLength(30)]
        [RegularExpression(@".*\S.*", ErrorMessage = "CRMV é obrigatório.")]
        public string Crmv { get; set; } = string.Empty;

        [Column("ES_ESPECIALIDADE")]
        [StringLength(100)]
        public string? Especialidade { get; set; }

        [Column("TL_TELEFONE")]
        [StringLength(20)]
        public string? Telefone { get; set; }

        [Column("EM_EMAIL")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [Column("AE_ATENDE_EMERGENCIA")]
        public bool AtendeEmergencia { get; set; }

        [Column("AT_ATIVO")]
        public bool Ativo { get; set; }

        [ForeignKey(nameof(Clinica))]
        [Column("ID_CLINICA")]
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
