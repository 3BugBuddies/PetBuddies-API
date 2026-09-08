namespace PetBuddies_API.Domain.Enums
{
    /// <summary>Resultado da avaliacao de uma prescricao num check-in. So DOSE_CALCULADA carrega dose.</summary>
    public enum DesfechoCheckinEnum
    {
        DOSE_CALCULADA,
        ACIONAR_CLINICA,
        SEM_DOSE
    }
}
