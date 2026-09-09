using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.RegraPontuacao
{
    public record RegraPontuacaoDto
    {
        public int Id { get; init; }
        public int ClinicaId { get; init; }
        public TipoGestoEnum Gesto { get; init; }
        public int Pontos { get; init; }
        public DateOnly InicioVigencia { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
