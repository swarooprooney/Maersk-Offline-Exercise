using Maersk.Sorting.Api.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.DataLayer
{
    public class MongoWrapper : IDatabaseWrapper
    {
        private readonly IMongoDatabase db;
        private readonly DBConfiguration _options;
        public MongoWrapper(IOptionsMonitor<DBConfiguration> dbConfig)
        {
            _options = dbConfig.CurrentValue;
            var mongoClient = new MongoClient(_options.ConnectionString);
            db = mongoClient.GetDatabase(_options.JobDatabase);
        }

        public async Task<bool> TryInsertNewRecordAsync<T>(string table, T record)
        {
            if (record != null)
            {
                var collection = db.GetCollection<T>(table);
                await collection.InsertOneAsync(record);
                return true;
            }
            return false;
        }

        public async Task<List<T>> GetAllRecordsAsync<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            var result = await collection.Find(new BsonDocument()).ToListAsync();
            return result;
        }

        public async Task<T> GetRecordByIdAsync<T>(string table, Guid guid)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", guid);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> UpdateRecord<T>(string table,Guid guid,T record)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", guid);
            var result = await collection.ReplaceOneAsync(filter,record);
            return result.ModifiedCount > 0 ? true : false;
        }
    }
}
