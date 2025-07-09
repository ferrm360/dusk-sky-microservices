using MongoDB.Driver;
using MongoDB.Bson;


namespace AddService.Api.Infrastructure;

public class MongoConnector
{
    private const int MaxRetries = 5;
    private const int DelayMilliseconds = 2000;

    public async Task<IMongoDatabase> ConnectWithRetriesAsync(string connectionString, string dbName)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(dbName);

                // Verificar conexión con ping
                var command = new BsonDocument("ping", 1);
                await database.RunCommandAsync<BsonDocument>(command);

                return database;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al conectar a MongoDB (intento {attempt}): {ex.Message}");

                if (attempt == MaxRetries)
                {
                    Console.WriteLine("🛑 Se alcanzó el número máximo de reintentos.");
                    throw;
                }

                await Task.Delay(DelayMilliseconds);
            }
        }

        throw new InvalidOperationException("Error inesperado al conectar a MongoDB.");
    }
}
