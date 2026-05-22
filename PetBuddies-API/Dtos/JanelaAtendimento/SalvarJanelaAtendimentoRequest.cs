using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.JanelaAtendimento
{
    public class SalvarJanelaAtendimentoRequest
    {
        [Required(ErrorMessage = "DataHoraInicio é obrigatória.")]
        public DateTime? DataHoraInicio { get; set; }

        [Required(ErrorMessage = "DataHoraFim é obrigatória.")]
        public DateTime? DataHoraFim { get; set; }

        [Range(1, 1440, ErrorMessage = "Duração do slot deve estar entre 1 e 1440 minutos.")]
        public int DuracaoSlot { get; set; }

        public int VeterinarioId { get; set; }
    }
}
