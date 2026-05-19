namespace PetBuddies_API.Dtos.JanelaAtendimento
{
    public class JanelaAtendimentoDto
    {
        public int Id { get; init; }

        public DateOnly Data { get; init; }

        public TimeOnly HoraInicio { get; init; }

        public TimeOnly HoraFim { get; init; }

        public int VeterinarioId { get; init; }

        public string VeterinarioNome { get; init; } = string.Empty;
    }
}
