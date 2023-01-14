namespace EasyApiSecurity.AuthorizationManager.Mongo;

public class EasyApiSecurityMongoDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ResourcesCollectionName { get; set; } = null!;
}