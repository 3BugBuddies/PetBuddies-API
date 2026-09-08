using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Responsavel
{
    public class CadastrarResponsavelRequest
    {
        [Required(ErrorMessage = "Nome do responsável é obrigatório.")]
        [StringLength(150, ErrorMessage = "Nome do responsável deve ter no máximo 150 caracteres.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do responsável é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone do responsável é obrigatório.")]
        [StringLength(20, ErrorMessage = "Telefone do responsável deve ter no máximo 20 caracteres.")]
        [RegularExpression(@".*\d.*", ErrorMessage = "Telefone do responsável deve conter ao menos um dígito.")]
        public string Telefone { get; set; } = string.Empty;
    }
}
