# Easy API security

The purpose of this project is to allow you to limit access to the resources of an API project without write the permissions in the source code.

The project is based on a middleware that inspects the calls and through the JWT token asks an authorization manager to verify if the user can actually access the resource he requested. 
The implementation of authorization manager is delegated to an external piece of code, so everyone can manage this type of control as you like.

This solution contains two projects: Core and Demo. The Demo project demonstrates how to use the Core component. 
Below we see how to integrate this component into an ASP.NET Core API project.

The first step is to configure the Core component

```c#
MiddlewareContext middlewareContext = new MiddlewareContext();
middlewareContext.AuthorizationManager = new DemoAuthorizationManager();
middlewareContext.JwtSettings = new JwtSettings() 
{ 
    Audience = "audience", 
    Issuer = "issuer", 
    Secret = "this is super secret" 
};

app.UseEas(middlewareContext);
```

In this example the information for the creation of the token is written in the code for convenience, in the correct use these values must be taken from a configuration file or even better from an HSM.

Within the configuration we also passed an instance of a class derived from IAuthorizationManager. The CanAccess method must be implemented within this class. 
The implementation of this method will be responsible for whether or not to authorize the access of a resource by a user.
This is the signature of the method:

```c#
bool CanAccess (JwtInformations informations, string resource, string method)
```

The first argument contains the information of the user who made the request to the resource (name, email, roles, etc.).
This information, together with the other two parameters (requested resource and method used) can be used to create a fairly complex and granular access logic. For example, users belonging to a specific role can be allowed to access GET, POST and DELETE, while other users belonging to different roles can only access the same resource in GET.

If a user is not authorized to access a resource, he will receive an error in response.

To create and obtain a valid token it is necessary to verify the user's credentials, in this demo we use username and password written in the code for brevity. 
Once the credentials have been verified it will be possible to create a valid token to be returned to the user. 
The user can use this token to make calls to protected resources, compatibly with his authorizations (implemented in the Authorization component).
The example below illustrates the demo implementation, obviously valid for demonstration purposes only:

```c#
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
```


Within a resource it will be possible to access the information of the user who made the call using the object JwtInformations.Current, as illustrated in the example below:

```c#
app.MapGet("/private", () => {

    return Results.Ok($"Hello {JwtInformations.Current.Name}!");

});
```


