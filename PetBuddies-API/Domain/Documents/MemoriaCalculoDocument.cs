using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Documents
{
    // Documento da coleção memorias_calculo (MongoDB): o que o Java mandou, as regras
    // vigentes usadas e quanto cada critério rendeu. O valor oficial é o lançamento no Oracle.
    public class MemoriaCalculoDocument
    {
        public string? Id { get; set; }

        public long ConsultaId { get; set; }

        public int LancamentoId { get; set; }

        public long ResponsavelId { get; set; }

        public long AnimalId { get; set; }

        public int ClinicaId { get; set; }

        public PeriodoCalculo Periodo { get; set; } = new();

        public EntradaCalculo Entrada { get; set; } = new();

        public List<ParcelaCalculo> Parcelas { get; set; } = new();

        public int Total { get; set; }

        public DateTime CalculadoEm { get; set; }
    }

    public class PeriodoCalculo
    {
        // Nulo na primeira consulta do animal: não há consulta anterior.
        public DateTime? De { get; set; }

        public DateTime Ate { get; set; }
    }

    public class EntradaCalculo
    {
        public int DosesPrevistas { get; set; }

        public int DosesComCheckin { get; set; }

        public int DiasNoPeriodo { get; set; }

        public int DiasComCheckin { get; set; }

        public int AcionamentosClinica { get; set; }

        public EvolucaoClinicaEnum? Evolucao { get; set; }
    }

    public class ParcelaCalculo
    {
        public TipoGestoEnum Gesto { get; set; }

        public int Quantidade { get; set; }

        public int PontosPorUnidade { get; set; }

        public int Pontos { get; set; }

        public int? RegraId { get; set; }

        // DateTime e não DateOnly: o driver 2.x do MongoDB não serializa DateOnly.
        public DateTime? InicioVigencia { get; set; }

        public string Observacao { get; set; } = string.Empty;
    }
}
