using MongoDB.Bson;
using MongoDB.Driver;

namespace GameListService.Api.Infrastructure
{
    /// <summary>
    /// Gestiona la conexión a MongoDB con reintentos para asegurar la disponibilidad.
    /// </summary>
    public class MongoConnector
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _delayBetweenRetries;

        public MongoConnector(int maxRetries = 10, int delayMilliseconds = 3000)
        {
            _maxRetries = maxRetries;
            _delayBetweenRetries = TimeSpan.FromMilliseconds(delayMilliseconds);
        }

        /// <summary>
        /// Intenta conectar a MongoDB con reintentos y retorna la base de datos.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión MongoDB.</param>
        /// <param name="databaseName">Nombre de la base de datos.</param>
        /// <param name="cancellationToken">Token para cancelar la operación.</param>
        /// <returns>Instancia de IMongoDatabase conectada.</returns>
        /// <exception cref="TimeoutException">Lanzada si no se logra conectar tras los intentos.</exception>
        public async Task<IMongoDatabase> ConnectWithRetriesAsync(string connectionString, string databaseName, CancellationToken cancellationToken = default)
        {
            var client = new MongoClient(connectionString);
            IMongoDatabase database;

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    database = client.GetDatabase(databaseName);
                    var command = new BsonDocument("ping", 1);
                    await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);

                    Console.WriteLine($"Conexión exitosa a MongoDB en intento {attempt}.");
                    return database;
                }
                catch (Exception ex) when (attempt < _maxRetries)
                {
                    Console.WriteLine($"Intento {attempt} fallido: {ex.Message}. Reintentando en {_delayBetweenRetries.TotalSeconds} segundos...");
                    await Task.Delay(_delayBetweenRetries, cancellationToken);
                }
            }

            throw new TimeoutException($"No se pudo conectar a MongoDB después de {_maxRetries} intentos.");
        }
    }
}
