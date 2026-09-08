using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.Procedimento
{
    public class SalvarProcedimentoRequest
    {
        [Required]
        public TipoProcedimentoEnum? Tipo { get; set; }

        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Descricao { get; set; }

        [Required]
        public StatusProcedimentoEnum? Status { get; set; }

        public DateTime? DataPrevistaInicio { get; set; }
        public DateTime? DataPrevistaFim { get; set; }

        [StringLength(500)]
        public string? AnexosUrl { get; set; }

        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "RegistroAtendimentoId deve ser maior que zero.")]
        public int RegistroAtendimentoId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "AnimalId deve ser maior que zero.")]
        public int AnimalId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "VeterinarioId deve ser maior que zero.")]
        public int VeterinarioId { get; set; }
    }
}
