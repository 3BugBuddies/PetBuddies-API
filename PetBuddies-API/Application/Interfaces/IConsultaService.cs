using PetBuddies_API.Application.Dtos.Consulta;

namespace PetBuddies_API.Application.Interfaces
{
    public interface IConsultaService
    {
        Task<bool> AnimalExisteAsync(int animalId);
        Task<bool> JanelaExisteAsync(int janelaId);
        Task<bool> JanelaOcupadaAsync(int janelaId, int? ignorarConsultaId = null);
        Task<bool> ConsultaRealizadaAsync(int consultaId);
        Task<ConsultaDto> AgendarAsync(AgendarConsultaRequest request);
        Task<List<ConsultaDto>> ListarAsync();
        Task<List<ConsultaDto>> ListarPorAnimalAsync(int animalId);
        Task<ConsultaDto?> BuscarPorIdAsync(int consultaId);
        Task<ConsultaDto?> CancelarAsync(int consultaId, AtualizarConsultaStatusRequest request);
        Task<ConsultaDto?> AtualizarAsync(int consultaId, AtualizarConsultaRequest request);
        Task<bool> RemoverAsync(int consultaId);
    }
}
