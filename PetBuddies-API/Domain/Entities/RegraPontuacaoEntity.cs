using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Entities
{
    /// <summary>
    /// Quanto vale cada gesto no programa Pata Segura (ADR s3-12).
    ///
    /// <para>Mesma vigência por sucessão da oferta, e a mesma consequência: nada de validar
    /// range. A unicidade aqui é simples — <c>(clínica, gesto, início)</c> —, porque nenhuma
    /// coluna da chave é nulável.</para>
    ///
    /// <para>Nesta sprint é política sem consumidor: nenhuma aplicação lê a tabela. O
    /// congelamento no fato é Sprint 4.</para>
    /// </summary>
    public class RegraPontuacaoEntity : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>Referência solta, sem FK — mesmo motivo de <c>OfertaEntity.ClinicaId</c>.</summary>
        public int ClinicaId { get; set; }

        public TipoGestoEnum Gesto { get; set; }

        /// <summary>Sempre positivo: gesto que tira ponto não existe no programa.</summary>
        public int Pontos { get; set; }

        public DateOnly InicioVigencia { get; set; }
    }
}
