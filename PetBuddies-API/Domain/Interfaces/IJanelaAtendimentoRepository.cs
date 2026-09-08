using PetBuddies_API.Domain.Entities;

namespace PetBuddies_API.Domain.Interfaces
{
    public interface IJanelaAtendimentoRepository
    {
        /// <summary>Leitura pura, com o veterinário hidratado para exibição.</summary>
        Task<JanelaAtendimentoEntity?> ObterPorIdAsync(int janelaId);

        /// <summary>Leitura pura sem <c>Include</c> — usada em checagens internas de ocupação
        /// e horário, que só precisam de VeterinarioId/DataHoraInicio.</summary>
        Task<JanelaAtendimentoEntity?> ObterParaVerificacaoAsync(int janelaId);

        /// <summary>Sem <c>AsNoTracking</c>: a entidade será alterada ou removida.</summary>
        Task<JanelaAtendimentoEntity?> ObterParaAlterarAsync(int janelaId);

        Task<bool> ExisteAsync(int janelaId);

        /// <summary>Veterinário não é domínio deste repositório — checagem preservada aqui
        /// enquanto não existir um IVeterinarioRepository.</summary>
        Task<bool> VeterinarioExisteAsync(int veterinarioId);

        Task<bool> HorarioExisteAsync(int veterinarioId, DateTime dataHoraInicio, int? ignorarJanelaId = null);

        Task<List<JanelaAtendimentoEntity>> ListarAsync();
        Task<List<JanelaAtendimentoEntity>> ListarFuturasAsync(DateTime agora);

        Task AdicionarAsync(JanelaAtendimentoEntity janela);
        void Remover(JanelaAtendimentoEntity janela);
    }
}
