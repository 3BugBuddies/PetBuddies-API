using PetBuddies_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PetBuddies_API.Models
{
    [Table("pb_tb_animal")]
    public class AnimalEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength]
        public string Nome { get; set; }

        public SexoEnum Sexo { get; set; }

        public DateOnly DataNascimento { get; set; }

        public decimal Peso { get; set; }

        public bool CondicaoCronica { get; set; }

        public bool PreCadastro { get; set; }

        public bool Castrado { get; set; }

        public string Foto { get; set; } // Verificar se fica melhor como byte[]

        public DateTime CreatedAt { get; set; }


        [ForeignKey(nameof(ResponsavelEntity))]
        public int ResponsavelId { get; set; }

        [JsonIgnore]
        public ResponsavelEntity? Responsavel { get; set; }

        [ForeignKey(nameof(TipoAnimalEntity))]
        public int TipoAnimalId { get; set; }

        [JsonIgnore]
        public TipoAnimalEntity? TipoAnimal { get; set; }


        public ProntuarioEntity? Prontuario { get; set; }


        public ICollection<RegistroAtendimentoEntity> RegistroAtendimentos { get; set; }


        public ICollection<ConsultaEntity> Consultas { get; set; }


        public ICollection<ProcedimentoEntity> Procedimentos { get; set; }
    }
}
