using PetBuddies_API.Application.Dtos.Protocolo;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class ProtocoloMapper
    {
        public static ProtocoloDto ToDto(this ProtocoloEntity protocolo)
        {
            return new ProtocoloDto
            {
                Id = protocolo.Id,
                Nome = protocolo.Nome,
                Categoria = protocolo.Categoria,
                Especie = protocolo.Especie,
                Ativo = protocolo.Ativo,
                Descricao = protocolo.Descricao,
                CreatedAt = protocolo.CreatedAt,
                Regras = protocolo.Regras
                    .OrderBy(regra => regra.Id)
                    .Select(regra => regra.ToDto())
                    .ToList()
            };
        }

        public static void Aplicar(this ProtocoloEntity protocolo, SalvarProtocoloRequest request)
        {
            protocolo.Nome = request.Nome.Trim();
            protocolo.Categoria = request.Categoria!.Value;
            protocolo.Especie = request.Especie!.Value;
            protocolo.Ativo = request.Ativo;
            protocolo.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
        }
    }
}
