namespace PetBuddies_API.Infrastructure.Mongo
{
    public class MongoDbOptions
    {
        // Variável de ambiente: MongoDb__ConnectionString (e MongoDb__Database).
        public const string SecaoConfiguracao = "MongoDb";

        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        public string Database { get; set; } = "petbuddies";
    }
}
