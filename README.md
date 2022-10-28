# Easy API security

The purpose of this project is to allow you to limit access to the resources of an API project without write the permissions in the source code.

The project is based on a middleware that inspects the calls and through the JWT token asks an authorization manager to verify if the user can actually access the resource he requested. 
The implementation of authorization manager is delegated to an external piece of code, so everyone can manage this type of control as you like.

This solution contains two projects: Core and Demo. The Demo project demonstrates how to use the Core component. 
Below we see how to integrate this component into an ASP.NET Core API project.

The first step is to configure the Core component

```c#
MiddlewareContext middlewareContext = new MiddlewareContext();
middlewareContext.Storage = new DemoAuthorizationManager();
middlewareContext.JwtSettings = new JwtSettings() 
{ 
    Audience = "audience", 
    Issuer = "issuer", 
    Secret = "this is super secret" 
};

app.UseEas(middlewareContext);
```