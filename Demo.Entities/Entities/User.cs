using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.Entities.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Token { get; set; }

    public string Email {  get; set; }

    public bool IsPasswordUpdated { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTime RefreshTokenExpiry { get; set; }
}
