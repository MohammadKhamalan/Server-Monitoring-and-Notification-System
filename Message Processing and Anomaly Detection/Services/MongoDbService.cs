using Message_Processing_and_Anomaly_Detection.Interfaces;
using Message_Processing_and_Anomaly_Detection.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_Processing_and_Anomaly_Detection.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<ServerStatistics> _collection;
        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _collection = database.GetCollection<ServerStatistics>(configuration["MongoDB:CollectionName"]);
        }
        public async Task InsertAsync(ServerStatistics statistics)
        {
            await _collection.InsertOneAsync(statistics);
        }
    }
}
