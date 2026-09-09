using PetBuddies_API.Domain.Enums;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// O que a clínica cobra por um ato, e desde quando.
    ///
    /// <para><b>A vigência é por sucessão</b>: só existe <c>DT_INICIO_VIGENCIA</c>, e um preço
    /// vale até o próximo começar. Não há intervalo, logo não há sobreposição possível — e não
    /// há validação de range a escrever.</para>
    ///
    /// <para><b>A referência ao ato mora em duas colunas</b>, das quais exatamente uma é
    /// preenchida. Tipo de procedimento e tipo de consulta são enums com <c>CHECK</c>, não
    /// tabelas: não há catálogo para apontar, e daí <see cref="Subtipo"/>. Protocolo é linha,
    /// e daí <see cref="ProtocoloId"/>. O <c>CK_OFERTA_ALVO</c> garante a exclusividade.</para>
    /// </summary>
    public class OfertaEntity : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Referência solta, sem FK: <c>T_PB_CLINICA</c> sai do .NET quando o Java absorver o
        /// registro, e a tabela não pode ficar apontando para o que não existe mais.
        /// </summary>
        public int ClinicaId { get; set; }

        public TipoAtoOfertaEnum Ato { get; set; }

        /// <summary>Valor do enum do ato quando <see cref="Ato"/> é CONSULTA ou PROCEDIMENTO; nulo em PROTOCOLO.</summary>
        public string? Subtipo { get; set; }

        /// <summary>Preenchido só quando <see cref="Ato"/> é PROTOCOLO.</summary>
        public long? ProtocoloId { get; set; }

        [JsonIgnore]
        public ProtocoloEntity? Protocolo { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public decimal Valor { get; set; }

        public DateOnly InicioVigencia { get; set; }
    }
}
