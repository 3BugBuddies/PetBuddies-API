using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.Procedimento
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

        public int RegistroAtendimentoId { get; set; }
        public int AnimalId { get; set; }
        public int VeterinarioId { get; set; }
    }
}
