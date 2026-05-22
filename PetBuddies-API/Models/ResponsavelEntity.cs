using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("T_PB_RESPONSAVEL")]
    public class ResponsavelEntity : BaseEntity
    {
        [Key]
        [Column("ID_RESPONSAVEL")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do responsável é obrigatório.")]
        [Column("NM_NOME_RESPONSAVEL")]
        [StringLength(150)]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do responsável é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Column("NR_CPF")]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 dígitos.")]
        public string? Cpf { get; set; }

        [Column("DT_DATA_NASCIMENTO")]
        public DateOnly? DataNascimento { get; set; }

        [Required(ErrorMessage = "Telefone do responsável é obrigatório.")]
        [Column("TL_TELEFONE")]
        [StringLength(20)]
        [RegularExpression(@".*\d.*", ErrorMessage = "Telefone deve conter ao menos um dígito.")]
        public string Telefone { get; set; } = string.Empty;

        [Column("EM_EMAIL")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [Column("ST_STATUS")]
        [EnumDataType(typeof(StatusTutorEnum))]
        public StatusTutorEnum Status { get; set; }

        [ForeignKey(nameof(Clinica))]
        [Column("ID_CLINICA")]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }

        [ForeignKey(nameof(Endereco))]
        [Column("ID_ENDERECO")]
        public int? EnderecoId { get; set; }

        [JsonIgnore]
        public EnderecoEntity? Endereco { get; set; }

        [JsonIgnore]
        public ICollection<AnimalEntity> Animais { get; set; } = [];
    }
}
