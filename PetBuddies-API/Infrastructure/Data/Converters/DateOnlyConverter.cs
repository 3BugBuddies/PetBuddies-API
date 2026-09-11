using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PetBuddies_API.Infrastructure.Data.Converters
{
    /// <summary>
    /// Converte <see cref="DateOnly"/> para <see cref="DateTime"/> na ida ao banco, e de volta na
    /// leitura.
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
