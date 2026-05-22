namespace PetBuddies_API.Dtos.Prontuario
{
    public class ProntuarioDto
    {
        public int Id { get; set; }
        public string? Alergias { get; set; }
        public string? Observacoes { get; set; }
        public int AnimalId { get; set; }
    }
}
