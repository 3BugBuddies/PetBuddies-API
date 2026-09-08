using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Infrastructure.Clients
{
    /// <summary>
    /// O gatilho de entrada no serviço de cuidado (Java). Ambas as chamadas são
    /// best-effort: falha do Java não desfaz o cadastro clínico.
    ///
    /// <para>Existe como interface para poder ser dublada nos testes de caso de uso —
    /// sem isso, testar o cadastro de animal exigiria o serviço Java no ar.</para>
    /// </summary>
    public interface IMotorApiClient
    {
        Task InstanciarPlanoPreventivoAsync(
            int animalId,
            EspecieEnum especie,
            PorteEnum porte,
            SexoEnum sexo,
            bool castrado,
            DateOnly dataNascimento);

        Task InstanciarPlanoPosCirurgicoAsync(int animalId, int consultaId);
    }
}
