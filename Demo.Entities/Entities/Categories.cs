using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Demo.Entities.Entities;

public class Categories
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Type { get; set; }
}
