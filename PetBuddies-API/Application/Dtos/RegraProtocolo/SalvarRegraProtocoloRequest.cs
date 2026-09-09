using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.RegraProtocolo
{
    public record SalvarRegraProtocoloRequest
    {
        [Range(1, long.MaxValue, ErrorMessage = "ProtocoloId deve ser maior que zero.")]
        public long ProtocoloId { get; init; }

        [Required(ErrorMessage = "Tipo de cuidado é obrigatório.")]
        public TipoCuidadoEnum? Tipo { get; init; }

        [Required(ErrorMessage = "Nome da regra é obrigatório.")]
        [StringLength(255)]
        public string Nome { get; init; } = string.Empty;

        /// <summary>Quanto somar à data-base. Zero significa "no próprio dia".</summary>
        [Required(ErrorMessage = "Deslocamento é obrigatório.")]
        [Range(0, 9999, ErrorMessage = "Deslocamento deve estar entre 0 e 9999.")]
        public int? Offset { get; init; }

        [Required(ErrorMessage = "Unidade do deslocamento é obrigatória.")]
        public UnidadeTempoEnum? UnidadeOffset { get; init; }

        [Required(ErrorMessage = "Data-base é obrigatória.")]
        public TipoDataBaseEnum? DataBase { get; init; }

        /// <summary>Nulo significa ocorrência única. Preenchido, exige a unidade.</summary>
        [Range(1, 9999, ErrorMessage = "Intervalo deve estar entre 1 e 9999.")]
        public int? Intervalo { get; init; }

        public UnidadeTempoEnum? UnidadeIntervalo { get; init; }

        /// <summary>
        /// Nunca nulo: "para sempre" vira número explícito, ou o motor entra em laço. Sem
        /// <c>[Required]</c> de propósito — a obrigatoriedade é invariante de domínio e mora no
        /// service, que é onde o teste unitário da rubrica a exercita.
        /// </summary>
        public int? Repeticoes { get; init; }

        [StringLength(2000)]
        public string? Descricao { get; init; }
    }
}
