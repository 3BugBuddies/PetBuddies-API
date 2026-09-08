namespace PetBuddies_API.Dtos.JanelaAtendimento
{
    public class JanelaAtendimentoDto
    {
        public int Id { get; init; }

        public DateTime DataHoraInicio { get; init; }

        public int VeterinarioId { get; init; }

        public string VeterinarioNome { get; init; } = string.Empty;

        /// <summary>A consulta que reservou o slot, ou nulo se o horário está livre.</summary>
        public int? ConsultaId { get; init; }
    }
}
