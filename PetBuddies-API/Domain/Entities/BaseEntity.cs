using System.ComponentModel.DataAnnotations.Schema;

namespace PetBuddies_API.Models
{
    public abstract class BaseEntity
    {
        [Column("CA_CREATED_AT")]
        public DateTime CreatedAt { get; set; }

        [Column("AT_UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }
    }
}
