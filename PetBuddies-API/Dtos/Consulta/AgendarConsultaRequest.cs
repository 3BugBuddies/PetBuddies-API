using System.ComponentModel.DataAnnotations;
using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.Consulta
{
    public class AgendarConsultaRequest
    {
        public int AnimalId { get; set; }

        public int JanelaId { get; set; }

        [Required(ErrorMessage = "TipoConsulta é obrigatório.")]
        [EnumDataType(typeof(TipoConsultaEnum), ErrorMessage = "Tipo de consulta inválido.")]
        public TipoConsultaEnum? TipoConsulta { get; set; }
    }
}
