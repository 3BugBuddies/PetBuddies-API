using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.Protocolo
{
    /// <summary>
    /// O protocolo com as regras dentro. É a forma que o motor do Java lê ao criar um plano:
    /// materializar os itens exige o molde inteiro, e uma segunda chamada por protocolo seria
    /// N+1 na fronteira entre os serviços.
    /// </summary>
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
