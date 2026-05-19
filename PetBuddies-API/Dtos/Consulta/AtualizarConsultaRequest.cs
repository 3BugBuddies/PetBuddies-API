using System.ComponentModel.DataAnnotations;
using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.Consulta
{
    public class AtualizarConsultaRequest
    {
        public int AnimalId { get; set; }

        public int JanelaId { get; set; }

        [Required(ErrorMessage = "TipoConsulta é obrigatório.")]
        [EnumDataType(typeof(TipoConsultaEnum), ErrorMessage = "Tipo de consulta inválido.")]
        public TipoConsultaEnum? TipoConsulta { get; set; }

        [Required(ErrorMessage = "Status da consulta é obrigatório.")]
        [EnumDataType(typeof(StatusConsultaEnum), ErrorMessage = "Status de consulta inválido.")]
        public StatusConsultaEnum? Status { get; set; }

        [StringLength(2000, ErrorMessage = "Observação deve ter no máximo 2000 caracteres.")]
        public string? Observacao { get; set; }
    }
}
