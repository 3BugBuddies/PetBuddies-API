using PetBuddies_API.Application.Dtos.RegraProtocolo;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class RegraProtocoloMapper
    {
        public static RegraProtocoloDto ToDto(this RegraProtocoloEntity regra)
        {
            return new RegraProtocoloDto
            {
                Id = regra.Id,
                ProtocoloId = regra.ProtocoloId,
                Tipo = regra.Tipo,
                Nome = regra.Nome,
                Offset = regra.Offset,
                UnidadeOffset = regra.UnidadeOffset,
                DataBase = regra.DataBase,
                Intervalo = regra.Intervalo,
                UnidadeIntervalo = regra.UnidadeIntervalo,
                Repeticoes = regra.Repeticoes,
                Descricao = regra.Descricao
            };
        }

        public static void Aplicar(this RegraProtocoloEntity regra, SalvarRegraProtocoloRequest request)
        {
            regra.ProtocoloId = request.ProtocoloId;
            regra.Tipo = request.Tipo!.Value;
            regra.Nome = request.Nome.Trim();
            regra.Offset = request.Offset!.Value;
            regra.UnidadeOffset = request.UnidadeOffset!.Value;
            regra.DataBase = request.DataBase!.Value;
            regra.Intervalo = request.Intervalo;
            regra.UnidadeIntervalo = request.UnidadeIntervalo;
            regra.Repeticoes = request.Repeticoes!.Value;
            regra.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
        }
    }
}
