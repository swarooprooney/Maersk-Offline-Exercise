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
        public MongoWrapper(string connectionString, string database)
        {
            var mongoClient = new MongoClient(connectionString);
            db = mongoClient.GetDatabase(database);
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
    }
}
