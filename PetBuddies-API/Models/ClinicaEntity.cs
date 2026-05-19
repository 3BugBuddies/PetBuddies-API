using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_clinica")]
    public class ClinicaEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength]
        public string Nome { get; set; }

        [Column("s_cnpj")]
        public string Cnpj { get; set; }

        [Column("s_telefone")]
        public string Telefone { get; set; }

        [Column("s_email")]
        [EmailAddress]
        public string Email { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }


        [ForeignKey(nameof(EnderecoEntity))]
        public int EnderecoId { get; set; }

        [JsonIgnore]
        public EnderecoEntity? Endereco { get; set; }


        public ICollection<ResponsavelEntity> Responsaveis { get; set; }

        public ICollection<ConsultaEntity> Consultas { get; set; }

        public ICollection<VeterinarioEntity> Veterinarios { get; set; }


    }
}
