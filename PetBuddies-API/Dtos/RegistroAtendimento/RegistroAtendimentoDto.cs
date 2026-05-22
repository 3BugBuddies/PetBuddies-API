namespace PetBuddies_API.Dtos.RegistroAtendimento
{
    public class RegistroAtendimentoDto
    {
        public int Id { get; set; }
        public DateTime DataAtendimento { get; set; }
        public string? Anamnese { get; set; }
        public string? Diagnostico { get; set; }
        public string? Tratamento { get; set; }
        public string? Observacao { get; set; }
        public int AnimalId { get; set; }
        public int ProntuarioId { get; set; }
        public int ConsultaId { get; set; }
    }
}
