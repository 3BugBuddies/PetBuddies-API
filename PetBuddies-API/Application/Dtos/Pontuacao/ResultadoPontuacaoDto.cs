namespace PetBuddies_API.Application.Dtos.Pontuacao
{
    // JaExistia diferencia o primeiro envio (201) do reenvio da mesma consulta (200).
    public record ResultadoPontuacaoDto(LancamentoPontosDto Lancamento, bool JaExistia);
}
