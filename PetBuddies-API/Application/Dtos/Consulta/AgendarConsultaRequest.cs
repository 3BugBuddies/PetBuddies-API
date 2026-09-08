using System.ComponentModel.DataAnnotations;
using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.Consulta
{
    public class AgendarConsultaRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "AnimalId deve ser maior que zero.")]
        public int AnimalId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "JanelaId deve ser maior que zero.")]
        public int JanelaId { get; set; }

        [Required(ErrorMessage = "TipoConsulta é obrigatório.")]
        [EnumDataType(typeof(TipoConsultaEnum), ErrorMessage = "Tipo de consulta inválido.")]
        public TipoConsultaEnum? TipoConsulta { get; set; }
    }
}
