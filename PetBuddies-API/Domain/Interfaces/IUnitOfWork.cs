namespace PetBuddies_API.Domain.Interfaces
{
    /// <summary>
    /// Confirma no banco tudo que foi alterado pelos repositórios desde a última chamada.
    ///
    /// <para>Existe porque nenhum repositório chama <c>SaveChangesAsync</c> por conta própria.
    /// Se cada um salvasse ao gravar, o fechamento de atendimento do <c>N7</c> — que escreve
    /// registro, procedimentos, prescrições e regras — viraria várias transações, e uma falha
    /// no meio deixaria gravação parcial.</para>
    ///
    /// <para>É também o que permite dublar a confirmação nos testes de caso de uso: o
    /// <c>Mock&lt;IUnitOfWork&gt;</c> verifica que houve exatamente um commit.</para>
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SalvarAsync(CancellationToken cancellationToken = default);
    }
}
