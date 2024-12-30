using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ApiUser.Domain.Entities;

[BsonIgnoreExtraElements]
public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty; 

    public DateTime DateInsert { get; set; }
}
