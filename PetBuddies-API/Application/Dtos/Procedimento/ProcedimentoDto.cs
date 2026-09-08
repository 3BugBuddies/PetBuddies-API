using PetBuddies_API.Enums;

namespace PetBuddies_API.Dtos.Procedimento
{
    public class ProcedimentoDto
    {
        public int Id { get; set; }
        public TipoProcedimentoEnum Tipo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public StatusProcedimentoEnum Status { get; set; }
        public DateTime DataPrevistaInicio { get; set; }
        public DateTime DataPrevistaFim { get; set; }
        public string? AnexosUrl { get; set; }
        public string? Observacao { get; set; }
        public int RegistroAtendimentoId { get; set; }
        public int AnimalId { get; set; }
        public int VeterinarioId { get; set; }
    }
}
