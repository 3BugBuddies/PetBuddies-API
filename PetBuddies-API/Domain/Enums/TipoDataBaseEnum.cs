namespace PetBuddies_API.Domain.Enums
{
    /// <summary>
    /// Data-base a partir da qual o deslocamento da regra é contado.
    ///
    /// <para><c>NASCIMENTO</c> usa a data de nascimento do animal; <c>DATA_CIRURGIA</c> usa a
    /// data do procedimento cirúrgico; <c>ULTIMA_REALIZACAO</c> usa a última vez que aquele
    /// cuidado foi feito no animal, e é ela que expressa periodicidade sem materializar
    /// ocorrência nenhuma (ADR s3-24).</para>
    ///
    /// <para>Quem interpreta a data-base é o motor, no Java. Aqui ela é só catálogo.</para>
    /// </summary>
    public enum TipoDataBaseEnum
    {
        NASCIMENTO,
        DATA_CIRURGIA,
        ULTIMA_REALIZACAO
    }
}
