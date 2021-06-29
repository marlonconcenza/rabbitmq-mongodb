using Common.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IUserRepository
    {
        Task<ObjectId> Create(User user);
        Task<User> Get(ObjectId objectId);
        Task<User> Get(Guid uuid);
        Task<IEnumerable<User>> Get();
        Task<User> GetByName(string name);
        Task<bool> Update(ObjectId objectId, User user);
        Task<bool> Delete(ObjectId objectId);
    }
}
