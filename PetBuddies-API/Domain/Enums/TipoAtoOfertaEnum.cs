namespace PetBuddies_API.Domain.Enums
{
    /// <summary>
    /// O que a oferta precifica. <c>PROCEDIMENTO</c> e <c>CONSULTA</c> apontam para um enum
    /// (a coluna <c>TP_SUBTIPO</c>); <c>PROTOCOLO</c> aponta para uma linha do catálogo
    /// (<c>ID_PROTOCOLO</c>). É o que o <c>CK_OFERTA_ALVO</c> garante.
    /// </summary>
    public enum TipoAtoOfertaEnum
    {
        PROCEDIMENTO,
        CONSULTA,
        PROTOCOLO
    }
}
