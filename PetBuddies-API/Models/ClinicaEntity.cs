using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_clinica")]
    public class ClinicaEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("s_cnpj")]
        [StringLength(14, MinimumLength = 14)]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [Column("s_telefone")]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Column("s_email")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Endereco))]
        [Column("id_endereco")]
        public int EnderecoId { get; set; }

        [JsonIgnore]
        public EnderecoEntity? Endereco { get; set; }

        [JsonIgnore]
        public ICollection<ResponsavelEntity> Responsaveis { get; set; } = [];

        [JsonIgnore]
        public ICollection<ConsultaEntity> Consultas { get; set; } = [];

        [JsonIgnore]
        public ICollection<VeterinarioEntity> Veterinarios { get; set; } = [];
    }
}
