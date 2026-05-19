using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_endereco")]
    public class EnderecoEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("s_logradouro")]
        [StringLength(150)]
        public string Logradouro { get; set; } = string.Empty;

        [Required]
        [Column("s_numero")]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [Column("s_complemento")]
        [StringLength(100)]
        public string? Complemento { get; set; }

        [Column("s_bairro")]
        [StringLength(100)]
        public string? Bairro { get; set; }

        [Required]
        [Column("s_cidade")]
        [StringLength(100)]
        public string? Cidade { get; set; }

        [Required]
        [Column("s_estado")]
        [StringLength(2)]
        public string? Estado { get; set; }

        [Required]
        [Column("s_cep")]
        [StringLength(8)]
        public string? Cep { get; set; }

        [Required]
        [Column("s_pais")]
        [StringLength(60)]
        public string? Pais { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [JsonIgnore]
        public ICollection<ResponsavelEntity> Responsaveis { get; set; } = [];
    }
}
