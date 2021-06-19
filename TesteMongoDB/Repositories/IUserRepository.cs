using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteMongoDB.Models;

namespace TesteMongoDB.Repositories
{
    public interface IUserRepository
    {
        Task<ObjectId> Create(User user);
        Task<User> Get(ObjectId objectId);
        Task<IEnumerable<User>> Get();
        Task<User> GetByName(string name);
        Task<bool> Update(ObjectId objectId, User user);
        Task<bool> Delete(ObjectId objectId);
    }
}
