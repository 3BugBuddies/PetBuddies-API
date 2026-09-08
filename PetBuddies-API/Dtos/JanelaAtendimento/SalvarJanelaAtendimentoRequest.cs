using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.JanelaAtendimento
{
    public class SalvarJanelaAtendimentoRequest
    {
        [Required(ErrorMessage = "DataHoraInicio é obrigatória.")]
        public DateTime? DataHoraInicio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "VeterinarioId deve ser maior que zero.")]
        public int VeterinarioId { get; set; }
    }
}
