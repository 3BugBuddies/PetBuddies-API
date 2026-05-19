using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.JanelaAtendimento
{
    public class SalvarJanelaAtendimentoRequest
    {
        [Required(ErrorMessage = "Data da janela é obrigatória.")]
        public DateOnly? Data { get; set; }

        [Required(ErrorMessage = "Hora de início é obrigatória.")]
        public TimeOnly? HoraInicio { get; set; }

        [Required(ErrorMessage = "Hora de fim é obrigatória.")]
        public TimeOnly? HoraFim { get; set; }

        [Range(1, 1440, ErrorMessage = "Duração do slot deve estar entre 1 e 1440 minutos.")]
        public int DuracaoSlot { get; set; }

        public int VeterinarioId { get; set; }
    }
}
