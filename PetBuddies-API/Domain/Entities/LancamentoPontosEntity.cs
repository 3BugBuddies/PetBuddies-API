namespace PetBuddies_API.Domain.Entities
{
    // Quanto o responsável recebeu numa consulta. O saldo é a soma destas linhas;
    // o detalhe do cálculo fica na memória de cálculo, no MongoDB.
    public class LancamentoPontosEntity : BaseEntity
    {
        public int Id { get; set; }

        public long ConsultaId { get; set; }

        public long ResponsavelId { get; set; }

        public long AnimalId { get; set; }

        public int ClinicaId { get; set; }

        public DateTime DataConsulta { get; set; }

        public int Pontos { get; set; }
    }
}
