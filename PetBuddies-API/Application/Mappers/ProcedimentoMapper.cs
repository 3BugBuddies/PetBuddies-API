using PetBuddies_API.Application.Dtos.Procedimento;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class ProcedimentoMapper
    {
        public static ProcedimentoDto ToDto(this ProcedimentoEntity procedimento)
        {
            return new ProcedimentoDto
            {
                Id = procedimento.Id,
                Tipo = procedimento.Tipo,
                Nome = procedimento.Nome,
                Descricao = procedimento.Descricao,
                Status = procedimento.Status,
                DataPrevistaInicio = procedimento.DataPrevistaInicio,
                DataPrevistaFim = procedimento.DataPrevistaFim,
                AnexosUrl = procedimento.AnexosUrl,
                Observacao = procedimento.Observacao,
                RegistroAtendimentoId = procedimento.RegistroAtendimentoId,
                AnimalId = procedimento.AnimalId,
                VeterinarioId = procedimento.VeterinarioId
            };
        }

        public static void Aplicar(this ProcedimentoEntity procedimento, SalvarProcedimentoRequest request)
        {
            var dataInicio = request.DataPrevistaInicio ?? DateTime.Now;

            procedimento.Tipo = request.Tipo!.Value;
            procedimento.Nome = request.Nome.Trim();
            procedimento.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
            procedimento.Status = request.Status!.Value;
            procedimento.DataPrevistaInicio = dataInicio;
            procedimento.DataPrevistaFim = request.DataPrevistaFim ?? dataInicio;
            procedimento.AnexosUrl = string.IsNullOrWhiteSpace(request.AnexosUrl) ? null : request.AnexosUrl.Trim();
            procedimento.Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim();
            procedimento.RegistroAtendimentoId = request.RegistroAtendimentoId;
            procedimento.AnimalId = request.AnimalId;
            procedimento.VeterinarioId = request.VeterinarioId;
        }
    }
}
