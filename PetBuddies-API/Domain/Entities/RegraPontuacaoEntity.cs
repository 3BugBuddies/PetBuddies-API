using PetBuddies_API.Domain.Enums;

namespace PetBuddies_API.Domain.Entities
{
    public class RegraPontuacaoEntity : BaseEntity
    {
        public int Id { get; set; }

        public int ClinicaId { get; set; }

        public TipoGestoEnum Gesto { get; set; }

        public int Pontos { get; set; }

        public DateOnly InicioVigencia { get; set; }
    }
}
