using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EasyApiSecurity.AuthorizationManager.Mongo.Entities;

public abstract class Resource
{
    protected Resource()
    {
        Roles = new List<string>();
    }
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Url")] public string Url { get; set; } = null!;
    [BsonElement("Method")] public string Method { get; set; } = null!;
    [BsonElement("IsPublic")] public bool IsPublic { get; set; }
    [BsonElement("Roles")]
    public List<string> Roles { get; set; }
}