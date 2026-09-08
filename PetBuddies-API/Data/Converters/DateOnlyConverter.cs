using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PetBuddies_API.Data.Converters
{
    /// <summary>
    /// Converte <see cref="DateOnly"/> para <see cref="DateTime"/> na ida ao banco, e de volta na
    /// leitura.
    ///
    /// <para>Sem isto, o provider Oracle do EF Core não tem tradução nativa para
    /// <see cref="DateOnly"/> e cai no fallback de string: a coluna nasce
    /// <c>NVARCHAR2(10)</c> em vez de <c>DATE</c>. O sintoma não aparece em listagem,
    /// porque ISO <c>yyyy-MM-dd</c> ordena igual como texto — aparece em
    /// <c>TRUNC</c>, <c>MONTHS_BETWEEN</c>, <c>BETWEEN</c> com bind de data, e no
    /// índice único que sustenta a adesão do check-in.</para>
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter()
            : base(
                data => data.ToDateTime(TimeOnly.MinValue),
                valor => DateOnly.FromDateTime(valor))
        {
        }
    }
}
