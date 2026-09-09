using PetBuddies_API.Domain.Enums;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    public class ProtocoloEntity : BaseEntity
    {
        public long Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public CategoriaProtocoloEnum Categoria { get; set; }

        public EspecieEnum Especie { get; set; }

        public bool Ativo { get; set; } = true;

        public string? Descricao { get; set; }

        [JsonIgnore]
        public ICollection<RegraProtocoloEntity> Regras { get; set; } = new List<RegraProtocoloEntity>();
    }
}
