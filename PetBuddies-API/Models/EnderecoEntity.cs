using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_ENDERECO")]
    public class EnderecoEntity : BaseEntity
    {
        [Key]
        [Column("ID_ENDERECO")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Logradouro é obrigatório.")]
        [Column("LG_LOGRADOURO")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Logradouro é obrigatório.")]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório.")]
        [Column("NR_NUMERO")]
        [StringLength(20)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Número é obrigatório.")]
        public string Numero { get; set; } = string.Empty;

        [Column("CM_COMPLEMENTO")]
        [StringLength(100)]
        public string? Complemento { get; set; }

        [Column("BR_BAIRRO")]
        [StringLength(100)]
        public string? Bairro { get; set; }

        [Required(ErrorMessage = "Cidade é obrigatória.")]
        [Column("CD_CIDADE")]
        [StringLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estado é obrigatório.")]
        [Column("ES_ESTADO")]
        [StringLength(2, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-z]{2}$", ErrorMessage = "Estado deve conter a UF com 2 letras.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "CEP é obrigatório.")]
        [Column("NR_CEP")]
        [StringLength(8, MinimumLength = 8)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter 8 dígitos.")]
        public string Cep { get; set; } = string.Empty;

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [JsonIgnore]
        public ICollection<ResponsavelEntity> Responsaveis { get; set; } = [];
    }
}
