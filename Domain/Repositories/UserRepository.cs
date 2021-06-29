using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient client)
        {
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("MongoDataBase"));
            var collection = database.GetCollection<User>(nameof(User));
            _users = collection;
        }

        public async Task<ObjectId> Create(User user)
        {
            await _users.InsertOneAsync(user);
            return user.id;
        }

        public async Task<bool> Delete(ObjectId objectId)
        {
            var filter = Builders<User>.Filter.Eq(c => c.id, objectId);
            var result = await _users.DeleteOneAsync(filter);
            return result.DeletedCount == 1;
        }

        public async Task<User> Get(ObjectId objectId)
        {
            var filter = Builders<User>.Filter.Eq(c => c.id, objectId);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> Get()
        {
            return await _users.Find(x => true).ToListAsync();
        }

        public async Task<User> Get(Guid uuid)
        {
            var filter = Builders<User>.Filter.Eq(c => c.uuid, uuid);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetByName(string name)
        {
            return await _users.Find(x => x.name == name).FirstOrDefaultAsync();
        }

        public async Task<bool> Update(ObjectId objectId, User user)
        {
            var filter = Builders<User>.Filter.Eq(c => c.id, objectId);

            var update = Builders<User>.Update
                .Set(c => c.name, user.name)
                .Set(c => c.age, user.age);

            var result = await _users.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
    }
}
