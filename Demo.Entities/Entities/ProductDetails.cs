using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Demo.Entities.Entities;

public class ProductDetails
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("ProductName")]
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public int ProductPrice { get; set; }
    public int ProductStock { get; set; }
    public string? CategoryId { get; set; }
    public string? productType { get; set; }
    public List<string> productStatus { get; set; }
    public string? fromDate { get; set; }
    public string? toDate { get; set; }
    public string? time { get; set;}
    public string? fileUpload { get; set; }
}
