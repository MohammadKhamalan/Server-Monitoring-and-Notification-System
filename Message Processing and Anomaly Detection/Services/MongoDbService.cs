using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using MongoDB.Driver;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<ServerStatistics> _collection;

        public MongoDbService(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<ServerStatistics>(collectionName);
        }

        public async Task InsertAsync(ServerStatistics statistics)
        {
            await _collection.InsertOneAsync(statistics);
        }
    }
}