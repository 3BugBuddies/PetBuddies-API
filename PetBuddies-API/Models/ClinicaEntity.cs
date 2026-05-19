using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_CLINICA")]
    public class ClinicaEntity : BaseEntity
    {
        [Key]
        [Column("ID_CLINICA")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome da clínica é obrigatório.")]
        [Column("NM_NOME_CLINICA")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome da clínica é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNPJ da clínica é obrigatório.")]
        [Column("NR_CNPJ")]
        [StringLength(14, MinimumLength = 14)]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ da clínica deve conter 14 dígitos.")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone da clínica é obrigatório.")]
        [Column("TL_TELEFONE")]
        [StringLength(20)]
        [RegularExpression(@".*\d.*", ErrorMessage = "Telefone deve conter ao menos um dígito.")]
        public string Telefone { get; set; } = string.Empty;

        [Column("EM_EMAIL")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [ForeignKey(nameof(Endereco))]
        [Column("ID_ENDERECO")]
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
