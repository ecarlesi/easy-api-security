﻿using EasyApiSecurity.Core;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;

namespace EasyApiSecurity.AuthorizationManager.SqlServer
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private readonly string _connectionString;

        public AuthorizationManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool CanAccess(JwtInformations? informations, string resource, string method)
        {
            string cacheKey = $"{method}@{resource}";

            CacheItem? cacheItem = _cache.Get<CacheItem>(cacheKey);

            if (cacheItem == null)
            {
                cacheItem = LoadCacheItemFromDatabase(resource, method);

                _cache.Set(cacheKey, cacheItem, DateTime.Now.AddSeconds(60));
            }

            if (cacheItem!.IsPublic)
            {
                return true;
            }

            if (informations!.Roles == null) return false;

            return (from role in informations.Roles from r in cacheItem.Roles where role == r select role).Any();
        }

        private CacheItem? LoadCacheItemFromDatabase(string resource, string method)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string statement = @"
select * from [resource]
where 
IsPublic = 1
and [Url] = @r
and Method = @m;
;
";
                using (SqlCommand command = new SqlCommand(statement, connection))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@r", Value = resource });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@m", Value = method });

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CacheItem() { IsPublic = true, Path = resource, Method = method, Roles = new List<string>() };
                        }
                    }
                }

                statement = @"
select r.[name] from [role] r
join [resourceRole] rr on r.id = rr.roleId
join [resource] s on s.id = rr.resourceId
where 
s.[url] = @r
and s. method = @m
;
";
                using (SqlCommand command = new SqlCommand(statement, connection))
                {
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@r", Value = resource });
                    command.Parameters.Add(new SqlParameter() { ParameterName = "@m", Value = method });

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> roles = new List<string>();

                        while (reader.Read())
                        {
                            string role = reader.GetString(0);

                            roles.Add(role);
                        }

                        return new CacheItem() { IsPublic = false, Path = resource, Method = method, Roles = roles };
                    }
                }
            }
        }
    }
}

internal class CacheItem
{
    internal string? Path { get; set; }
    internal string? Method { get; set; }
    internal bool IsPublic { get; set; }
    internal List<string>? Roles { get; set; }
}
