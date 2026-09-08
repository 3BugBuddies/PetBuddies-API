namespace PetBuddies_API.Application.Dtos.Responsavel
{
    public class ResponsavelDto
    {
        public int Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string Telefone { get; init; } = string.Empty;

        public string? Email { get; init; }
    }
}
