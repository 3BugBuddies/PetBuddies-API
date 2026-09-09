using PetBuddies_API.Application.Dtos.RegraPontuacao;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class RegraPontuacaoMapper
    {
        public static RegraPontuacaoDto ToDto(this RegraPontuacaoEntity regra)
        {
            return new RegraPontuacaoDto
            {
                Id = regra.Id,
                ClinicaId = regra.ClinicaId,
                Gesto = regra.Gesto,
                Pontos = regra.Pontos,
                InicioVigencia = regra.InicioVigencia,
                CreatedAt = regra.CreatedAt,
                UpdatedAt = regra.UpdatedAt
            };
        }

        public static void Aplicar(this RegraPontuacaoEntity regra, SalvarRegraPontuacaoRequest request)
        {
            regra.ClinicaId = request.ClinicaId;
            regra.Gesto = request.Gesto!.Value;
            regra.Pontos = request.Pontos!.Value;
            regra.InicioVigencia = request.InicioVigencia!.Value;
        }
    }
}
