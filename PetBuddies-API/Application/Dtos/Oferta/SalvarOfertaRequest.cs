using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.Oferta
{
    /// <summary>
    /// A vigência é por sucessão: só <see cref="InicioVigencia"/> existe, e um preço vale até o
    /// próximo começar. Não há fim para informar, e portanto não há intervalo a sobrepor.
    /// </summary>
    public record SalvarOfertaRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ClinicaId deve ser maior que zero.")]
        public int ClinicaId { get; init; }

        [Required(ErrorMessage = "Tipo de ato é obrigatório.")]
        public TipoAtoOfertaEnum? Ato { get; init; }

        /// <summary>Obrigatório em CONSULTA e PROCEDIMENTO; nulo em PROTOCOLO.</summary>
        [StringLength(50)]
        public string? Subtipo { get; init; }

        /// <summary>Obrigatório em PROTOCOLO; nulo nos outros dois.</summary>
        public long? ProtocoloId { get; init; }

        [Required(ErrorMessage = "Descrição da oferta é obrigatória.")]
        [StringLength(255)]
        public string Descricao { get; init; } = string.Empty;

        [Required(ErrorMessage = "Valor da oferta é obrigatório.")]
        public decimal? Valor { get; init; }

        [Required(ErrorMessage = "Início da vigência é obrigatório.")]
        public DateOnly? InicioVigencia { get; init; }
    }
}
