using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PetBuddies_API.Infrastructure.Data.Converters
{
    /// <summary>
    /// A variante nulável de <see cref="DateOnlyConverter"/>.
    ///
    /// <para>Precisa existir separada: o EF resolve o conversor pelo tipo exato da
    /// propriedade, e <c>DateOnly?</c> não é <c>DateOnly</c>. Sem esta classe, as
    /// colunas opcionais — hoje <c>PR_PROXIMO_RETORNO</c> e <c>PR_PROXIMA_VACINA</c> —
    /// continuariam sendo gravadas como texto enquanto as obrigatórias já estariam
    /// corretas, que é a pior das situações: metade do schema de um jeito.</para>
    /// </summary>
    public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
    {
        public NullableDateOnlyConverter()
            : base(
                data => data.HasValue ? data.Value.ToDateTime(TimeOnly.MinValue) : null,
                valor => valor.HasValue ? DateOnly.FromDateTime(valor.Value) : null)
        {
        }
    }
}
