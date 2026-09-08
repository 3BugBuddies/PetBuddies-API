namespace PetBuddies_API.Application.Dtos.Animal
{
    /// <summary>Projeção que o motor de cuidado do serviço Java consome.</summary>
    public class AnimalMotorDto
    {
        public int Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public DateOnly DataNascimento { get; init; }

        public bool CondicaoCronica { get; init; }

        public bool Castrado { get; init; }

        public string Sexo { get; init; } = string.Empty;

        public string Especie { get; init; } = string.Empty;

        public string Raca { get; init; } = string.Empty;

        public string Porte { get; init; } = string.Empty;
    }
}
