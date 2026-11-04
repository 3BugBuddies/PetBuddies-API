using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using PetBuddies_API.Domain.Documents;

namespace PetBuddies_API.Infrastructure.Mongo
{
    // Mapeamento por código, e não por atributo, para o Domain não depender do driver.
    public static class MongoMapeamento
    {
        private static readonly object Trava = new();
        private static bool _registrado;

        public static void Registrar()
        {
            lock (Trava)
            {
                if (_registrado)
                {
                    return;
                }

                // camelCase nos campos e enum como texto, igual ao JSON da API.
                ConventionRegistry.Register(
                    "petbuddies-documentos",
                    new ConventionPack
                    {
                        new CamelCaseElementNameConvention(),
                        new EnumRepresentationConvention(BsonType.String),
                        new IgnoreExtraElementsConvention(true)
                    },
                    tipo => tipo.Namespace == typeof(MemoriaCalculoDocument).Namespace);

                BsonClassMap.RegisterClassMap<MemoriaCalculoDocument>(mapa =>
                {
                    mapa.AutoMap();
                    mapa.MapIdMember(memoria => memoria.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                });

                _registrado = true;
            }
        }
    }
}
