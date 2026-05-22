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

        public int AnimalId { get; set; }
        public int? ProntuarioId { get; set; }
        public int ConsultaId { get; set; }
    }
}
