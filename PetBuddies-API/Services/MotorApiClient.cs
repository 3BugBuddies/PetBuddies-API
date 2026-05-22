using System.Text;
using System.Text.Json;
using PetBuddies_API.Enums;

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

        public async Task InstanciarPlanoPreventivoAsync(int animalId, EspecieEnum especie, PorteEnum porte, SexoEnum sexo, bool castrado, DateOnly dataNascimento)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var body = JsonSerializer.Serialize(new
                {
                    petNetApiAnimalId = animalId,
                    especie = especie.ToString(),
                    porte = porte.ToString(),
                    sexo = sexo.ToString(),
                    castrado,
                    dataNascimento
                });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}/api/motor/planos/instanciar-preventivo", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Motor plano preventivo falhou: animalId={AnimalId} status={Status} body={Body}",
                        animalId, (int)response.StatusCode, errorBody);
                    return;
                }

                _logger.LogInformation("Motor plano preventivo: animalId={AnimalId} status={Status}",
                    animalId, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Motor indisponível ao instanciar plano para animal {AnimalId}: {Message}",
                    animalId, ex.Message);
            }
        }

        public async Task InstanciarPlanoPosCirurgicoAsync(int animalId, int consultaId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var body = JsonSerializer.Serialize(new
                {
                    petNetApiAnimalId = animalId,
                    petNetApiConsultaId = consultaId
                });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}/api/motor/planos/instanciar-pos-cirurgico", content);
                _logger.LogInformation("Motor plano pós-cirúrgico: animalId={AnimalId} consultaId={ConsultaId} status={Status}",
                    animalId, consultaId, (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Motor indisponível ao instanciar plano pós-cirúrgico para animal {AnimalId} consulta {ConsultaId}: {Message}",
                    animalId, consultaId, ex.Message);
            }
        }

        public async Task RecalcularScoreAsync(int animalId, string motivo)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var body = JsonSerializer.Serialize(new
                {
                    petNetApiAnimalId = animalId,
                    motivo
                });
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}/api/motor/scores/recalcular", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Motor recalcular score falhou: animalId={AnimalId} motivo={Motivo} status={Status} body={Body}",
                        animalId, motivo, (int)response.StatusCode, errorBody);
                    return;
                }

                _logger.LogInformation("Motor score recalculado: animalId={AnimalId} motivo={Motivo}",
                    animalId, motivo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Motor indisponível ao recalcular score para animal {AnimalId} motivo {Motivo}: {Message}",
                    animalId, motivo, ex.Message);
            }
        }
    }
}
