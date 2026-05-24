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

        [Range(1, int.MaxValue, ErrorMessage = "VeterinarioId deve ser maior que zero.")]
        public int VeterinarioId { get; set; }
    }
}
