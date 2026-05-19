namespace PetBuddies_API.Dtos.Responsavel
{
    public class ResponsavelDto
    {
        public int Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string Telefone { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;
    }
}
