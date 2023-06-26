To correctly use the mongo db implementation you should set this:

appsettings.json => 

  "EasyApiSecurityDatabase": {
        "ConnectionString": "mongodb://localhost:27017",
        "DatabaseName": "MyDatabase",
        "ResourcesCollectionName": "resources"
    }
    
This node in appsettings.json gives you the reference to the mongo instance with related database and collection to bind.

The values provided are only for example purpose.

In the Program.cs you should do like this:

#region configure the middleware

MiddlewareContext middlewareContext = new MiddlewareContext();
middlewareContext.AuthorizationManager = new MongoAuthorizeManager(builder.Configuration.Get<EasyApiSecurityMongoDatabaseSettings>("EasyApiSecurityDatabase"));
middlewareContext.JwtSettings = new JwtSettings()
{
    Audience = "audience",
    Issuer = "issuer",
    KeyType = KeyType.SymmetricKey,
    Key = Encoding.Default.GetBytes("this is super secret")
};

app.UseEas(middlewareContext);

#endregion