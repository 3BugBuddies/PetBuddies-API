using PetBuddies_API.Application.Dtos.Oferta;
using PetBuddies_API.Domain.Entities;
using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Mappers
{
    public static class OfertaMapper
    {
        public static OfertaDto ToDto(this OfertaEntity oferta)
        {
            return new OfertaDto
            {
                Id = oferta.Id,
                ClinicaId = oferta.ClinicaId,
                Ato = oferta.Ato,
                Subtipo = oferta.Subtipo,
                ProtocoloId = oferta.ProtocoloId,
                Descricao = oferta.Descricao,
                Valor = oferta.Valor,
                InicioVigencia = oferta.InicioVigencia,
                CreatedAt = oferta.CreatedAt,
                UpdatedAt = oferta.UpdatedAt
            };
        }

        public static void Aplicar(this OfertaEntity oferta, SalvarOfertaRequest request)
        {
            var ato = request.Ato!.Value;

            oferta.ClinicaId = request.ClinicaId;
            oferta.Ato = ato;
            oferta.Subtipo = ato == TipoAtoOfertaEnum.PROTOCOLO ? null : request.Subtipo!.Trim();
            oferta.ProtocoloId = ato == TipoAtoOfertaEnum.PROTOCOLO ? request.ProtocoloId : null;
            oferta.Descricao = request.Descricao.Trim();
            oferta.Valor = request.Valor!.Value;
            oferta.InicioVigencia = request.InicioVigencia!.Value;
        }
    }
}
