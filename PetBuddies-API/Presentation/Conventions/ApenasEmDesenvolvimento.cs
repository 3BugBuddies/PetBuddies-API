using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace PetBuddies_API.Presentation.Conventions
{
    /// <summary>Marca um controller que só deve existir no ambiente de desenvolvimento.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ApenasEmDesenvolvimentoAttribute : Attribute
    {
    }

    /// <summary>
    /// Tira os controllers marcados da descoberta do MVC. Eles não viram rota nem entram
    /// no Swagger porque o framework não chega a saber que existem — diferente de limpar
    /// seletores, que deixa o controller registrado e quebra a subida pelo ApiExplorer.
    /// </summary>
    public sealed class ControllersSemOsDeDesenvolvimento : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo tipo)
            => base.IsController(tipo)
               && !tipo.IsDefined(typeof(ApenasEmDesenvolvimentoAttribute), inherit: false);
    }
}
