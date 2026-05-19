using System.ComponentModel.DataAnnotations;
using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.Consulta
{
    public class AtualizarConsultaStatusRequest
    {
        [Required(ErrorMessage = "Status da consulta é obrigatório.")]
        [EnumDataType(typeof(StatusConsultaEnum), ErrorMessage = "Status de consulta inválido.")]
        public StatusConsultaEnum? Status { get; set; }

        [StringLength(2000, ErrorMessage = "Motivo deve ter no máximo 2000 caracteres.")]
        public string? Motivo { get; set; }
    }
}
