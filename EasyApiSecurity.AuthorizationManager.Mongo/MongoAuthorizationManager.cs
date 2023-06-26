using EasyApiSecurity.AuthorizationManager.Mongo.Entities;
using EasyApiSecurity.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EasyApiSecurity.AuthorizationManager.Mongo;

public class MongoAuthorizationManager : IAuthorizationManager
{
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    private readonly IMongoCollection<Resource> _resourcesCollection;

    public MongoAuthorizationManager(
        IOptions<EasyApiSecurityMongoDatabaseSettings> easyApiSecurityMongoDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            easyApiSecurityMongoDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            easyApiSecurityMongoDatabaseSettings.Value.DatabaseName);

        _resourcesCollection = mongoDatabase.GetCollection<Resource>(
            easyApiSecurityMongoDatabaseSettings.Value.ResourcesCollectionName);
    }

    public bool CanAccess(JwtInformations? informations, string resource, string method)
    {
        var cacheKey = $"{method}@{resource}";

        var cacheItem = _cache.Get<CacheItem?>(cacheKey);

        if (cacheItem == null)
        {
            cacheItem = LoadCacheItemFromDatabase(resource, method);

            _cache.Set(cacheKey, cacheItem, DateTime.Now.AddSeconds(60));
        }

        if (cacheItem is { IsPublic: true })
        {
            return true;
        }

        if (cacheItem?.Roles == null || cacheItem.Roles.Count == 0)
        {
            return false;
        }

        return informations!.Roles!.Intersect(cacheItem.Roles).Any();
    }

    private CacheItem? LoadCacheItemFromDatabase(string resource, string method)
    {
        var resourceFromDatabase = _resourcesCollection.Find(x =>
            string.Equals(x.Url, resource, StringComparison.InvariantCultureIgnoreCase)
            && string.Equals(x.Method, method, StringComparison.InvariantCultureIgnoreCase)
            && x.IsPublic).FirstOrDefault();

        return resourceFromDatabase != null
            ? new CacheItem()
            {
                IsPublic = resourceFromDatabase.IsPublic, Path = resource, Method = method,
                Roles = resourceFromDatabase.Roles
            }
            : null;
    }
}

internal class CacheItem
{
    internal string Path { get; set; } = null!;
    internal string Method { get; set; } = null!;
    internal bool IsPublic { get; set; }
    internal List<string> Roles { get; set; } = new List<string>();
}