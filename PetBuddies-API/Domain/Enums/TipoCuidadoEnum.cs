namespace PetBuddies_API.Domain.Enums
{
    /// <summary>
    /// O vocabulário de cuidados que a regra do protocolo descreve. É o mesmo enum que o
    /// item do plano usa no Java: a regra descreve o que o item vai ser.
    /// </summary>
    public enum TipoCuidadoEnum
    {
        VACINACAO,
        VERMIFUGACAO,
        EXAME,
        RETORNO,
        CIRURGIA,
        MEDICACAO,
        HIGIENE
    }
}
