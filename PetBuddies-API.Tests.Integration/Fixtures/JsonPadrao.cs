using System.Text.Json;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Tests.Integration.Fixtures
{
    // Espelha as opções que o Program.cs registra em AddJsonOptions: sem isto, o
    // HttpClient do teste serializa enum como número e o servidor exige string
    // (JsonStringEnumConverter allowIntegerValues: false) — todo POST volta 400.
    public static class JsonPadrao
    {
        public static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter(allowIntegerValues: false) }
        };
    }
}
