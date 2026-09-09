using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.RegraProtocolo
{
    public record RegraProtocoloDto
    {
        public long Id { get; init; }
        public long ProtocoloId { get; init; }
        public TipoCuidadoEnum Tipo { get; init; }
        public string Nome { get; init; } = string.Empty;
        public int Offset { get; init; }
        public UnidadeTempoEnum UnidadeOffset { get; init; }
        public TipoDataBaseEnum DataBase { get; init; }
        public int? Intervalo { get; init; }
        public UnidadeTempoEnum? UnidadeIntervalo { get; init; }
        public int Repeticoes { get; init; }
        public string? Descricao { get; init; }
    }
}
