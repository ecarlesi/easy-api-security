using EasyApiSecurity.Core;
using EasyApiSecurity.Demo;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region configure the middleware

MiddlewareContext middlewareContext = new MiddlewareContext();
middlewareContext.AuthorizationManager = new DemoAuthorizationManager();
middlewareContext.JwtSettings = new JwtSettings() 
{ 
    Audience = "audience", 
    Issuer = "issuer", 
    Secret = "this is super secret" 
};

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



