using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.Oferta
{
    public record OfertaDto
    {
        public int Id { get; init; }
        public int ClinicaId { get; init; }
        public TipoAtoOfertaEnum Ato { get; init; }
        public string? Subtipo { get; init; }
        public long? ProtocoloId { get; init; }
        public string Descricao { get; init; } = string.Empty;
        public decimal Valor { get; init; }
        public DateOnly InicioVigencia { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
