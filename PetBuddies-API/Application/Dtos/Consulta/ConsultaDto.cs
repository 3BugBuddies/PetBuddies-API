namespace PetBuddies_API.Dtos.Consulta
{
    public class ConsultaDto
    {
        public int Id { get; init; }

        public string TipoConsulta { get; init; } = string.Empty;

        public DateTime DataHora { get; init; }

        public string Status { get; init; } = string.Empty;

        public int AnimalId { get; init; }
    }
}
