using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.Protocolo
{
    public record ProtocoloDto
    {
        public long Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public CategoriaProtocoloEnum Categoria { get; init; }
        public EspecieEnum Especie { get; init; }
        public bool Ativo { get; init; }
        public string? Descricao { get; init; }
        public DateTime CreatedAt { get; init; }
        public IReadOnlyList<RegraProtocoloDto> Regras { get; init; } = Array.Empty<RegraProtocoloDto>();
    }
}
