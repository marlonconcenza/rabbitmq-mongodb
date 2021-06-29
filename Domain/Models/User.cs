using MongoDB.Bson;
using System;

namespace Common.Models
{
    public class User
    {
        public ObjectId id { get; set; }
        public Guid uuid { get; set; }
        public string name { get; set; }
        public int age { get; set; }
    }
}
