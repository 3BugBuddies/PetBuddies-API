using MongoDB.Driver;
using PetBuddies_API.Domain.Documents;
using PetBuddies_API.Domain.Interfaces;

namespace PetBuddies_API.Infrastructure.Mongo
{
    public class MemoriaCalculoRepository : IMemoriaCalculoRepository
    {
        public const string Colecao = "memorias_calculo";

        private readonly IMongoCollection<MemoriaCalculoDocument> _colecao;

        // Índices criados no primeiro uso, não na subida: a API sobe mesmo com o MongoDB fora.
        private volatile bool _indicesCriados;

        public MemoriaCalculoRepository(IMongoDatabase database)
        {
            _colecao = database.GetCollection<MemoriaCalculoDocument>(Colecao);
        }

        public async Task SalvarAsync(MemoriaCalculoDocument memoria, CancellationToken cancellationToken = default)
        {
            await GarantirIndicesAsync(cancellationToken);

            // Upsert pela consulta: regravar a mesma consulta substitui, nunca duplica.
            await _colecao.ReplaceOneAsync(
                documento => documento.ConsultaId == memoria.ConsultaId,
                memoria,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);
        }

        public async Task<MemoriaCalculoDocument?> ObterPorConsultaAsync(long consultaId, CancellationToken cancellationToken = default)
        {
            return await _colecao
                .Find(documento => documento.ConsultaId == consultaId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task GarantirIndicesAsync(CancellationToken cancellationToken)
        {
            if (_indicesCriados)
            {
                return;
            }

            var chaves = Builders<MemoriaCalculoDocument>.IndexKeys;
            await _colecao.Indexes.CreateManyAsync(
                new[]
                {
                    new CreateIndexModel<MemoriaCalculoDocument>(
                        chaves.Ascending(documento => documento.ConsultaId),
                        new CreateIndexOptions { Name = "ux_consulta", Unique = true }),
                    new CreateIndexModel<MemoriaCalculoDocument>(
                        chaves.Ascending(documento => documento.ResponsavelId),
                        new CreateIndexOptions { Name = "ix_responsavel" })
                },
                cancellationToken);

            _indicesCriados = true;
        }
    }
}
