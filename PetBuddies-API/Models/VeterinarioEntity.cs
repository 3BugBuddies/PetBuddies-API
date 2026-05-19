using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_veterinario")]
    public class VeterinarioEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        public string Nome { get; set; }

        public string Crmv { get; set; }

        public string Especialidade { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public bool AtendeEmergencia { get; set; }

        public bool Ativo { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ClinicaEntity))]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }


        public ICollection<ConsultaEntity> Consultas { get; set; }

        public ICollection<ProcedimentoEntity> Procedimentos { get; set; }

        public ICollection<JanelaAtendimentoEntity> janelaAtendimentos { get; set; }



    }
}
