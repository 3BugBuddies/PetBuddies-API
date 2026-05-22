namespace PetBuddies_API.Dtos.JanelaAtendimento
{
    public class JanelaAtendimentoDto
    {
        public int Id { get; init; }

        public DateTime DataHoraInicio { get; init; }

        public DateTime DataHoraFim { get; init; }

        public int DuracaoSlot { get; init; }

        public int VeterinarioId { get; init; }

        public string VeterinarioNome { get; init; } = string.Empty;
    }
}
