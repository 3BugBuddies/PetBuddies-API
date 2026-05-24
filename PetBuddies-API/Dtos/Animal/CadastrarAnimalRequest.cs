using System.ComponentModel.DataAnnotations;
using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.Animal
{
    public class CadastrarAnimalRequest
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

        [Required(ErrorMessage = "Porte do animal é obrigatório.")]
        [EnumDataType(typeof(PorteEnum), ErrorMessage = "Porte do animal inválido.")]
        public PorteEnum? Porte { get; set; }

        [Required(ErrorMessage = "Sexo do animal é obrigatório.")]
        [EnumDataType(typeof(SexoEnum), ErrorMessage = "Sexo do animal inválido.")]
        public SexoEnum? Sexo { get; set; }

        public bool Castrado { get; set; }

        [Required(ErrorMessage = "Data de nascimento do animal é obrigatória.")]
        public DateOnly? DataNascimento { get; set; }

    }
}
