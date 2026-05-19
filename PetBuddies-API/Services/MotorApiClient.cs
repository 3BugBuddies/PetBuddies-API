using System.Text;
using System.Text.Json;

namespace PetBuddies_API.Services
{
    public class MotorApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        private readonly ILogger<MotorApiClient> _logger;

        public MotorApiClient(IHttpClientFactory httpClientFactory,
                              IConfiguration configuration,
                              ILogger<MotorApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration["MotorApi:BaseUrl"] ?? "http://localhost:8080";
            _logger = logger;
        }

        public async Task InstanciarPlanoPreventivoAsync(int animalId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var body = JsonSerializer.Serialize(new { petNetApiAnimalId = animalId });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}/api/motor/planos/instanciar", content);
                _logger.LogInformation("Motor plano preventivo: animalId={AnimalId} status={Status}",
                    animalId, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Motor indisponível ao instanciar plano para animal {AnimalId}: {Message}",
                    animalId, ex.Message);
            }
        }
    }
}
