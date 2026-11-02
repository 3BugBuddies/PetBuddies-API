using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.Pontuacao
{
    // Enviado pelo Java ao fechar a consulta: os fatos do período entre a consulta
    // anterior e esta. As regras e o cálculo ficam deste lado.
    public record PontuarConsultaRequest
    {
        [Required(ErrorMessage = "ConsultaId é obrigatório.")]
        public long? ConsultaId { get; init; }

        [Required(ErrorMessage = "ResponsavelId é obrigatório.")]
        public long? ResponsavelId { get; init; }

        [Required(ErrorMessage = "AnimalId é obrigatório.")]
        public long? AnimalId { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "ClinicaId deve ser maior que zero.")]
        public int ClinicaId { get; init; }

        [Required(ErrorMessage = "Data da consulta é obrigatória.")]
        public DateTime? DataConsulta { get; init; }

        // Nula na primeira consulta do animal.
        public DateTime? DataConsultaAnterior { get; init; }

        [Required(ErrorMessage = "Doses previstas são obrigatórias.")]
        public int? DosesPrevistas { get; init; }

        [Required(ErrorMessage = "Doses com check-in são obrigatórias.")]
        public int? DosesComCheckin { get; init; }

        [Required(ErrorMessage = "Dias no período são obrigatórios.")]
        public int? DiasNoPeriodo { get; init; }

        [Required(ErrorMessage = "Dias com check-in são obrigatórios.")]
        public int? DiasComCheckin { get; init; }

        [Required(ErrorMessage = "Acionamentos da clínica são obrigatórios.")]
        public int? AcionamentosClinica { get; init; }

        // Opcional: sem avaliação do veterinário, o critério de evolução não pontua.
        public EvolucaoClinicaEnum? Evolucao { get; init; }
    }
}
