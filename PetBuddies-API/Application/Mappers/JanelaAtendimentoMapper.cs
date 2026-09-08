using PetBuddies_API.Application.Dtos.JanelaAtendimento;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class JanelaAtendimentoMapper
    {
        public static JanelaAtendimentoDto ToDto(this JanelaAtendimentoEntity janela)
        {
            return new JanelaAtendimentoDto
            {
                Id = janela.Id,
                DataHoraInicio = janela.DataHoraInicio,
                VeterinarioId = janela.VeterinarioId,
                VeterinarioNome = janela.Veterinario?.Nome ?? string.Empty,
                ConsultaId = janela.ConsultaId
            };
        }
    }
}
