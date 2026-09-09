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

        [Required(ErrorMessage = "Deslocamento é obrigatório.")]
        [Range(0, 9999, ErrorMessage = "Deslocamento deve estar entre 0 e 9999.")]
        public int? Offset { get; init; }

        [Required(ErrorMessage = "Unidade do deslocamento é obrigatória.")]
        public UnidadeTempoEnum? UnidadeOffset { get; init; }

        [Required(ErrorMessage = "Data-base é obrigatória.")]
        public TipoDataBaseEnum? DataBase { get; init; }

        [Range(1, 9999, ErrorMessage = "Intervalo deve estar entre 1 e 9999.")]
        public int? Intervalo { get; init; }

        public UnidadeTempoEnum? UnidadeIntervalo { get; init; }

        public int? Repeticoes { get; init; }

        [StringLength(2000)]
        public string? Descricao { get; init; }
    }
}
