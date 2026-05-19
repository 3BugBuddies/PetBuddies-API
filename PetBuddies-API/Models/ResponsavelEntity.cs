using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_responsavel")]
    public class ResponsavelEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("s_cpf")]
        [StringLength(11, MinimumLength = 11)]
        public string Cpf { get; set; } = string.Empty;

        [Column("dt_nascimento")]
        public DateOnly DataNascimento { get; set; }

        [Required]
        [Column("s_telefone")]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Column("s_email")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [Column("st_responsavel")]
        public StatusTutorEnum Status { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Clinica))]
        [Column("id_clinica")]
        
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [ForeignKey(nameof(Endereco))]
        [Column("id_endereco")]
        public int? EnderecoId { get; set; }

        [JsonIgnore]
        public EnderecoEntity? Endereco { get; set; }

        [JsonIgnore]
        public ICollection<AnimalEntity> Animais { get; set; } = [];
    }
}
