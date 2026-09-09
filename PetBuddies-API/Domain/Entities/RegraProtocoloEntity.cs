using PetBuddies_API.Domain.Enums;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// O molde de um item de cuidado dentro de um protocolo.
    ///
    /// <para>O agendamento é expresso por data-base + deslocamento + recorrência, e não por
    /// campos fixos: a data-base diz de onde contar, o deslocamento diz quanto somar, e o
    /// intervalo com as repetições diz quantas vezes o item volta. Intervalo nulo significa
    /// ocorrência única.</para>
    ///
    /// <para><c>Repeticoes</c> é obrigatório: "para sempre" vira número explícito, porque o
    /// plano não tem fim e um nulo aqui geraria expansão infinita no motor (ADR s3-24).</para>
    /// </summary>
    /// <remarks>
    /// Não herda <c>BaseEntity</c>: a tabela não tem <c>CA_CREATED_AT</c> nem
    /// <c>AT_UPDATED_AT</c> (<c>01_ddl.sql:661</c>).
    /// </remarks>
    public class RegraProtocoloEntity
    {
        public long Id { get; set; }

        public long ProtocoloId { get; set; }

        [JsonIgnore]
        public ProtocoloEntity Protocolo { get; set; } = null!;

        public TipoCuidadoEnum Tipo { get; set; }

        public string Nome { get; set; } = string.Empty;

        /// <summary>Quanto somar à data-base para a primeira ocorrência.</summary>
        public int Offset { get; set; }

        public UnidadeTempoEnum UnidadeOffset { get; set; }

        public TipoDataBaseEnum DataBase { get; set; }

        /// <summary>Nulo significa ocorrência única. Preenchido, exige <see cref="UnidadeIntervalo"/>.</summary>
        public int? Intervalo { get; set; }

        public UnidadeTempoEnum? UnidadeIntervalo { get; set; }

        /// <summary>Quantas vezes o item ocorre, contando a primeira. Nunca nulo, nunca menor que 1.</summary>
        public int Repeticoes { get; set; }

        public string? Descricao { get; set; }
    }
}
