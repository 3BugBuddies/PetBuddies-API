using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Application.Dtos.Pontuacao
{
    public record MemoriaCalculoDto
    {
        public long ConsultaId { get; init; }
        public int LancamentoId { get; init; }
        public long ResponsavelId { get; init; }
        public long AnimalId { get; init; }
        public int ClinicaId { get; init; }
        public DateTime? PeriodoDe { get; init; }
        public DateTime PeriodoAte { get; init; }
        public int DosesPrevistas { get; init; }
        public int DosesComCheckin { get; init; }
        public int DiasNoPeriodo { get; init; }
        public int DiasComCheckin { get; init; }
        public int AcionamentosClinica { get; init; }
        public EvolucaoClinicaEnum? Evolucao { get; init; }
        public List<ParcelaCalculoDto> Parcelas { get; init; } = new();
        public int Total { get; init; }
        public DateTime CalculadoEm { get; init; }
    }

    public record ParcelaCalculoDto
    {
        public TipoGestoEnum Gesto { get; init; }
        public int Quantidade { get; init; }
        public int PontosPorUnidade { get; init; }
        public int Pontos { get; init; }
        public int? RegraId { get; init; }
        public DateOnly? InicioVigencia { get; init; }
        public string Observacao { get; init; } = string.Empty;
    }
}
