using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Dtos.RegistroAtendimento
{
    public class SalvarRegistroAtendimentoRequest
    {
        public DateTime? DataAtendimento { get; set; }

        [StringLength(2000)]
        public string? Anamnese { get; set; }

        [StringLength(2000)]
        public string? Diagnostico { get; set; }

        [StringLength(2000)]
        public string? Tratamento { get; set; }

        [StringLength(2000)]
        public string? Observacao { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AnimalId deve ser maior que zero.")]
        public int AnimalId { get; set; }
        public int? ProntuarioId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ConsultaId deve ser maior que zero.")]
        public int ConsultaId { get; set; }
    }
}
