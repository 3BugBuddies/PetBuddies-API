using PetBuddies_API.Domain.Enums;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    public class RegraProtocoloEntity
    {
        public long Id { get; set; }

        public long ProtocoloId { get; set; }

        [JsonIgnore]
        public ProtocoloEntity Protocolo { get; set; } = null!;

        public TipoCuidadoEnum Tipo { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int Offset { get; set; }

        public UnidadeTempoEnum UnidadeOffset { get; set; }

        public TipoDataBaseEnum DataBase { get; set; }

        public int? Intervalo { get; set; }

        public UnidadeTempoEnum? UnidadeIntervalo { get; set; }

        public int Repeticoes { get; set; }

        public string? Descricao { get; set; }
    }
}
