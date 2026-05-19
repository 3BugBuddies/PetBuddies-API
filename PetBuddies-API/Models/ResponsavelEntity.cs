using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_responsavel")]
    public class ResponsavelEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Cpf { get; set; }
        [Required]
        public string Telefone { get; set; }

        public string Email { get; set; }

        public StatusTutorEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ClinicaEntity))]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }
        

        [ForeignKey(nameof(EnderecoEntity))]
        public int EnderecoId { get; set; }

        [JsonIgnore]
        public EnderecoEntity? Endereco { get; set; }


        public ICollection<AnimalEntity> Animais { get; set; }

    }
}
