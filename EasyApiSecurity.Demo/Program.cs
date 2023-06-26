using EasyApiSecurity.Core;
using EasyApiSecurity.Demo;
using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region configure the middleware

MiddlewareContext middlewareContext = new MiddlewareContext
{
    AuthorizationManager = new DemoAuthorizationManager(),
    JwtSettings = new JwtSettings()
    {
        Audience = "audience",
        Issuer = "issuer",
        KeyType = KeyType.SymmetricKey,
        Key = Encoding.Default.GetBytes("this is super secret")
    }
};

app.UseEas(middlewareContext);

#endregion

app.MapGet("/", () => "Hello World!");

app.MapGet("/private", () => Results.Ok($"Hello {JwtInformations.Current!.Name}!"));

app.MapPost("/login", ([FromBody]LoginRequest loginRequest)  => {
    if (loginRequest.Username != "eca" || loginRequest.Password != "password") return Results.Unauthorized();
    JwtInformations informations = new JwtInformations
    {
        Name = "eca",
        Email = "emiliano.carlesi@gmail.com",
        Roles = new[] { "admin", "backup", "superhero" }
    };

    string token = JwtProvider.Instance().CreateToken(informations);

    return Results.Json(new LoginResponse() { Token = token });

});

app.Run();



