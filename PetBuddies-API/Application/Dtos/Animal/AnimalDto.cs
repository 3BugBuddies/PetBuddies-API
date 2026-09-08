namespace PetBuddies_API.Application.Dtos.Animal
{
    public class AnimalDto
    {
        public int Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string Especie { get; init; } = string.Empty;

        public string Raca { get; init; } = string.Empty;

        public string Porte { get; init; } = string.Empty;

        public string Sexo { get; init; } = string.Empty;

        public bool Castrado { get; init; }

        public bool CondicaoCronica { get; init; }

        public DateOnly DataNascimento { get; init; }

        public decimal? Peso { get; init; }

        /// <summary>Veio do prontuário, que foi fundido no animal.</summary>
        public string? Alergias { get; init; }

        /// <summary>Veio do prontuário, que foi fundido no animal.</summary>
        public string? Observacoes { get; init; }
    }
}
