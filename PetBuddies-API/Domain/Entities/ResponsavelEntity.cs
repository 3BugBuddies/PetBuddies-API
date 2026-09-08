using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
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

        [Required(ErrorMessage = "Telefone do responsável é obrigatório.")]
        [Column("TL_TELEFONE")]
        [StringLength(20)]
        [RegularExpression(@".*\d.*", ErrorMessage = "Telefone deve conter ao menos um dígito.")]
        public string Telefone { get; set; } = string.Empty;

        [Column("EM_EMAIL")]
        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }

        [JsonIgnore]
        public ICollection<AnimalEntity> Animais { get; set; } = [];
    }
}
