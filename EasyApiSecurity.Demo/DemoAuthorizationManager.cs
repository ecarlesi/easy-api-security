using EasyApiSecurity.Core;

namespace EasyApiSecurity.Demo
{
    public class DemoAuthorizationManager : IAuthorizationManager
    {
        public bool CanAccess(JwtInformations? informations, string resource, string method)
        {
            return resource != "/private" || (informations is { Roles: { } } && informations.Roles.Any(x => x == "admin"));
        }
    }
}