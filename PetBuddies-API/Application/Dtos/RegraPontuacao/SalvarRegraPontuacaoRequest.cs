using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.RegraPontuacao
{
    public record SalvarRegraPontuacaoRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ClinicaId deve ser maior que zero.")]
        public int ClinicaId { get; init; }

        [Required(ErrorMessage = "Gesto é obrigatório.")]
        public TipoGestoEnum? Gesto { get; init; }

        [Required(ErrorMessage = "Pontos são obrigatórios.")]
        public int? Pontos { get; init; }

        [Required(ErrorMessage = "Início da vigência é obrigatório.")]
        public DateOnly? InicioVigencia { get; init; }
    }
}
