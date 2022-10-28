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

By implementing customized AuthorizationManagers you will be able to adapt the component to your infrastructure by using different systems for managing credentials (eg database, ldap, Active Directory, external services, files, etc.). 
The DemoAuthorizationManager class shows a simple implementation valid for demo use only.

```c#
    public class DemoAuthorizationManager : IAuthorizationManager
    {
        public bool CanAccess(JwtInformations informations, string resource, string method)
        {
            if (resource == "/private")
            {
                if (informations != null && informations.Roles != null && informations.Roles.Where(x => x == "admin").FirstOrDefault() != null)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return true;
            }
        }
    }
```

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


Once the repo has been cloned and the solution compiled, start debugging the Demo project. Within the Demo project there is the demo.http file which must be used within Visual Studio Code after downloading the HTTP client extension. This extension allows you to make HTTP calls. Of course it is possible to use other products (eg Postman).

The demo.http file contains three calls to three distinct resources. 

The first call is to an unsecured resource. It can be called freely without a token.

![image](https://user-images.githubusercontent.com/195652/198606513-b18d5b70-7f21-4477-8397-0d00b5c3b6e0.png)

The second call, also without authorization, is the login. 
The username and password are specified in the request json (LoginRequest class in the Demo project). If the credentials are correct in the response message (LoginResponse class in the Demo project) we will find a valid JWT token. We will use this token in the next call to authenticate.

![image](https://user-images.githubusercontent.com/195652/198609855-510a9e17-566c-4bd0-9ff5-c1a72ed64955.png)

The third call is restricted to using a valid token. Before making the call it is therefore necessary to replace the text "<your token here>" with the token we obtained after login. Once this is done we can make the call.

![image](https://user-images.githubusercontent.com/195652/198611063-5dc07811-8ab7-4ce4-b051-19d3235253a8.png)

If in response instead of the greeting message we receive an error it could be that we have copied the token wrong, perhaps adding a character, a space and also copying the quotation marks, as in the case below.

![image](https://user-images.githubusercontent.com/195652/198613790-febb166b-ebcf-429b-a89d-67ec42d699cd.png)

