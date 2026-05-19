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
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("s_nome")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("tp_sexo")]
        public SexoEnum Sexo { get; set; }

        [Column("dt_nascimento")]
        public DateOnly DataNascimento { get; set; }

        [Column("nr_peso")]
        [Range(0, 999.99)]
        public decimal Peso { get; set; }

        [Column("bl_condicao_cronica")]
        public bool CondicaoCronica { get; set; }

        [Column("bl_pre_cadastro")]
        public bool PreCadastro { get; set; }

        [Column("bl_castrado")]
        public bool Castrado { get; set; }

        [Column("s_foto")]
        [StringLength(500)]
        public string? Foto { get; set; }

        [Column("dt_criado")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Responsavel))]
        [Column("id_responsavel")]
        public int ResponsavelId { get; set; }

        [JsonIgnore]
        public ResponsavelEntity? Responsavel { get; set; }

        [ForeignKey(nameof(TipoAnimal))]
        [Column("id_tipo_animal")]
        public int TipoAnimalId { get; set; }

        [JsonIgnore]
        public TipoAnimalEntity? TipoAnimal { get; set; }

        [JsonIgnore]
        public ProntuarioEntity? Prontuario { get; set; }

        [JsonIgnore]
        public ICollection<RegistroAtendimentoEntity> RegistroAtendimentos { get; set; } = [];

        [JsonIgnore]
        public ICollection<ConsultaEntity> Consultas { get; set; } = [];

        [JsonIgnore]
        public ICollection<ProcedimentoEntity> Procedimentos { get; set; } = [];
    }
}
