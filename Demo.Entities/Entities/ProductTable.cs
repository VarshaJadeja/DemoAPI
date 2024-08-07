using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Entities
{
    public class ProductTable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int userId { get; set; }
        public int productId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
