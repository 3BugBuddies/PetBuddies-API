using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_endereco")]
    public class EnderecoEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Logradouro { get; set; }

        [Required]
        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string Cep { get; set; }

        public string Pais { get; set; }



        [ForeignKey(nameof(ClinicaEntity))]
        public int ClinicaId { get; set; }

        [JsonIgnore]
        public ClinicaEntity? Clinica { get; set; }
    }
}
