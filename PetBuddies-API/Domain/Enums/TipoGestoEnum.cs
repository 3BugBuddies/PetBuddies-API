namespace PetBuddies_API.Domain.Enums
{
    /// <summary>
    /// Os quatro gestos pontuáveis do programa Pata Segura (ADR s3-12). Todos já existem
    /// com data no schema, sem coluna nova — o saldo é agregação sobre esses fatos.
    /// </summary>
    public enum TipoGestoEnum
    {
        PLANO_CRIADO,
        CONSULTA_AGENDADA,
        CONSULTA_REALIZADA,
        PROCEDIMENTO_EXECUTADO
    }
}
