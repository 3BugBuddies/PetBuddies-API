namespace PetBuddies_API.Application.Dtos.Pontuacao
{
    public record SaldoPontosDto
    {
        public long ResponsavelId { get; init; }
        public int Saldo { get; init; }
    }
}
