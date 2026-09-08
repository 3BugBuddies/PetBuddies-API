using System.ComponentModel.DataAnnotations;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.Animal
{
    public class AtualizarAnimalRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ResponsavelId deve ser maior que zero.")]
        public int ResponsavelId { get; set; }

        [Required(ErrorMessage = "Nome do animal é obrigatório.")]
        [StringLength(150, ErrorMessage = "Nome do animal deve ter no máximo 150 caracteres.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nome do animal é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Espécie do animal é obrigatória.")]
        [EnumDataType(typeof(EspecieEnum), ErrorMessage = "Espécie do animal inválida.")]
        public EspecieEnum? Especie { get; set; }

        /// <summary>
        /// Passou a ser pedida no cadastro. Antes o campo nascia vazio porque o bot
        /// não tinha como perguntar; com a fusão do tipo de animal, raça é do
        /// indivíduo e o app do vet pede.
        /// </summary>
        [StringLength(100, ErrorMessage = "Raça deve ter no máximo 100 caracteres.")]
        public string Raca { get; set; } = "SEM_RACA";

        [Required(ErrorMessage = "Porte do animal é obrigatório.")]
        [EnumDataType(typeof(PorteEnum), ErrorMessage = "Porte do animal inválido.")]
        public PorteEnum? Porte { get; set; }

        [Required(ErrorMessage = "Sexo do animal é obrigatório.")]
        [EnumDataType(typeof(SexoEnum), ErrorMessage = "Sexo do animal inválido.")]
        public SexoEnum? Sexo { get; set; }

        public bool Castrado { get; set; }

        public bool CondicaoCronica { get; set; }

        [Required(ErrorMessage = "Data de nascimento do animal é obrigatória.")]
        public DateOnly? DataNascimento { get; set; }

        [StringLength(2000, ErrorMessage = "Alergias deve ter no máximo 2000 caracteres.")]
        public string? Alergias { get; set; }

        [StringLength(2000, ErrorMessage = "Observações deve ter no máximo 2000 caracteres.")]
        public string? Observacoes { get; set; }
    }
}
