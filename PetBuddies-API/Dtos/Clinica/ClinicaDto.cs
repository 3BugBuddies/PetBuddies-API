namespace PetBuddies_API.Dtos.Clinica
{
    public class ClinicaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int EnderecoId { get; set; }
    }
}
