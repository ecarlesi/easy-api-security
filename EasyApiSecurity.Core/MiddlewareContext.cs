namespace EasyApiSecurity.Core
{
    public delegate void MiddlewareErrorHandler(string path, string method, Exception e);

    public class MiddlewareContext
    {
        public IAuthorizationManager AuthorizationManager { get; init; } = null!;
        public JwtSettings? JwtSettings { get; init; }
        public MiddlewareErrorHandlerBehavior ErrorHandlerBehavior { get; set; }
    }
}
