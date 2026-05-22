namespace PetBuddies_API.Dtos.Veterinario
{
    public class VeterinarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Crmv { get; set; } = string.Empty;
        public string? Especialidade { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public bool AtendeEmergencia { get; set; }
        public bool Ativo { get; set; }
        public int ClinicaId { get; set; }
    }
}
