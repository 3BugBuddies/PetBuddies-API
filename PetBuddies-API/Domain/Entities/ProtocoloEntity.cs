using PetBuddies_API.Domain.Enums;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// O catálogo de cuidado da clínica: o pacote que um veterinário aplica a um paciente.
    ///
    /// <para>Deixou de escolher sozinho no ADR s3-24 — saiu o matching por perfil do pet e
    /// sobrou <c>ES_ESPECIE</c> como único filtro. O veterinário aplica; o sistema não decide
    /// por ele.</para>
    ///
    /// <para>Passou do Java para cá pelo ADR s3-25: catálogo é política da clínica, não
    /// operação. O Java lê por HTTP, e só no momento de criar um plano.</para>
    /// </summary>
    /// <remarks>
    /// Mapeamento em <c>Infrastructure/Data/Configurations/ProtocoloConfiguration</c>.
    /// Herda <c>BaseEntity</c> pelo <c>CA_CREATED_AT</c>; <c>AT_UPDATED_AT</c> é ignorado na
    /// configuração porque a tabela não tem a coluna (<c>01_ddl.sql:634</c>).
    /// </remarks>
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
