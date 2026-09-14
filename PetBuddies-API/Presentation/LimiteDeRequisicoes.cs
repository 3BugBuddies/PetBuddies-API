namespace PetBuddies_API.Presentation
{
    public static class LimiteDeRequisicoes
    {
        public const string Politica = "politica_200_por_minuto";

        public const int PermissoesPorJanela = 200;

        public static readonly TimeSpan Janela = TimeSpan.FromMinutes(1);
    }
}
