namespace PetBuddies_API.Application.Dtos.Pontuacao
{
    public record LancamentoPontosDto
    {
        public int Id { get; init; }
        public long ConsultaId { get; init; }
        public long ResponsavelId { get; init; }
        public long AnimalId { get; init; }
        public int ClinicaId { get; init; }
        public DateTime DataConsulta { get; init; }
        public int Pontos { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
