using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.Oferta
{
    public record SalvarOfertaRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ClinicaId deve ser maior que zero.")]
        public int ClinicaId { get; init; }

        [Required(ErrorMessage = "Tipo de ato é obrigatório.")]
        public TipoAtoOfertaEnum? Ato { get; init; }

        [StringLength(50)]
        public string? Subtipo { get; init; }

        public long? ProtocoloId { get; init; }

        [Required(ErrorMessage = "Descrição da oferta é obrigatória.")]
        [StringLength(255)]
        public string Descricao { get; init; } = string.Empty;

        [Required(ErrorMessage = "Valor da oferta é obrigatório.")]
        public decimal? Valor { get; init; }

        [Required(ErrorMessage = "Início da vigência é obrigatório.")]
        public DateOnly? InicioVigencia { get; init; }
    }
}
