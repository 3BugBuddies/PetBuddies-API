using PetBuddies_API.Domain.Enums;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    public class OfertaEntity : BaseEntity
    {
        public int Id { get; set; }

        public int ClinicaId { get; set; }

        public TipoAtoOfertaEnum Ato { get; set; }

        public string? Subtipo { get; set; }

        public long? ProtocoloId { get; set; }

        [JsonIgnore]
        public ProtocoloEntity? Protocolo { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public DateOnly InicioVigencia { get; set; }
    }
}
