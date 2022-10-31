using EasyApiSecurity.Core;
using EasyApiSecurity.Demo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region configure the middleware

string connectionString = "Data Source = 192.168.1.10; Initial Catalog = Demo; User Id = demoUser; Password = demoPassword";

MiddlewareContext middlewareContext = new MiddlewareContext();
middlewareContext.AuthorizationManager = new EasyApiSecurity.AuthorizationManager.SqlServer.AuthorizationManager(connectionString);
middlewareContext.JwtSettings = new JwtSettings()
{
    Audience = "audience",
    Issuer = "issuer",
    KeyType = KeyType.SymmetricKey,
    Key = Encoding.Default.GetBytes("this is super secret")
};

middlewareContext.ErrorHandlerBehavior = MiddlewareErrorHandlerBehavior.ShowGeneric;

app.UseEas(middlewareContext);

#endregion

app.MapGet("/", () => "Hello World!");

app.MapGet("/private", () => {

    return Results.Ok($"Hello {JwtInformations.Current.Name}!");

});

app.MapPost("/login", ([FromBody]LoginRequest loginRequest)  => {

    if (loginRequest.Username == "eca" && loginRequest.Password == "password")
    {
        JwtInformations informations = new JwtInformations();

        informations.Name = "eca";
        informations.Email = "emiliano.carlesi@gmail.com";
        informations.Roles = new string[] { "admin", "backup", "superhero" };

        string token = JwtProvider.Instance().CreateToken(informations);

        return Results.Json(new LoginResponse() { Token = token });
    }

    return Results.Unauthorized();

});

app.Run();



