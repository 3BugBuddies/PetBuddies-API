namespace PetBuddies_API.Domain.Enums
{
    /// <summary>
    /// Filtro de aplicação do protocolo: que moldes o veterinário vê quando quer aplicar
    /// um preventivo ou um pós-cirúrgico.
    ///
    /// <para>Não admite <c>TRATAMENTO</c> de propósito — tratamento nasce de prescrição
    /// assinada, não de política da clínica, e portanto não tem molde (ADR s3-24 §4b).</para>
    /// </summary>
    public enum CategoriaProtocoloEnum
    {
        PREVENTIVO,
        POS_CIRURGICO
    }
}
