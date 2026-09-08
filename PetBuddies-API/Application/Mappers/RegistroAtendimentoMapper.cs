using PetBuddies_API.Application.Dtos.RegistroAtendimento;
using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Application.Mappers
{
    public static class RegistroAtendimentoMapper
    {
        public static RegistroAtendimentoDto ToDto(this RegistroAtendimentoEntity registro)
        {
            return new RegistroAtendimentoDto
            {
                Id = registro.Id,
                DataAtendimento = registro.DataAtendimento,
                Anamnese = registro.Anamnese,
                Diagnostico = registro.Diagnostico,
                Tratamento = registro.Tratamento,
                Observacao = registro.Observacao,
                AnimalId = registro.AnimalId,
                ConsultaId = registro.ConsultaId
            };
        }

        public static void Aplicar(this RegistroAtendimentoEntity registro, SalvarRegistroAtendimentoRequest request)
        {
            registro.DataAtendimento = request.DataAtendimento ?? DateTime.Now;
            registro.Anamnese = string.IsNullOrWhiteSpace(request.Anamnese) ? null : request.Anamnese.Trim();
            registro.Diagnostico = string.IsNullOrWhiteSpace(request.Diagnostico) ? null : request.Diagnostico.Trim();
            registro.Tratamento = string.IsNullOrWhiteSpace(request.Tratamento) ? null : request.Tratamento.Trim();
            registro.Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim();
            registro.AnimalId = request.AnimalId;
            registro.ConsultaId = request.ConsultaId;
        }
    }
}
