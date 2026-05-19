namespace PetBuddies_API.Dtos.Animal
{
    public class AnimalDto
    {
        public int Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string Especie { get; init; } = string.Empty;

        public string Porte { get; init; } = string.Empty;

        public string Sexo { get; init; } = string.Empty;

        public bool Castrado { get; init; }

        public bool PreCadastro { get; init; }
    }
}
