using PetBuddies_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PetBuddies_API.Application.Dtos.Protocolo
{
    /// <summary>
    /// Só o cabeçalho do protocolo. As regras entram e saem pelo próprio recurso — corpo
    /// aninhado exigiria decidir aqui o que fazer com regra ausente numa atualização.
    /// </summary>
    public record SalvarProtocoloRequest
    {
        [Required(ErrorMessage = "Nome do protocolo é obrigatório.")]
        [StringLength(255)]
        public string Nome { get; init; } = string.Empty;

        [Required(ErrorMessage = "Categoria do protocolo é obrigatória.")]
        public CategoriaProtocoloEnum? Categoria { get; init; }

        [Required(ErrorMessage = "Espécie do protocolo é obrigatória.")]
        public EspecieEnum? Especie { get; init; }

        public bool Ativo { get; init; } = true;

        [StringLength(2000)]
        public string? Descricao { get; init; }
    }
}
