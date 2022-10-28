using EasyApiSecurity.Core;

namespace EasyApiSecurity.Demo
{
    public class DemoAuthorizationManager : IAuthorizationManager
    {
        public bool CanAccess(JwtInformations informations, string resource, string method)
        {
            if (resource == "private" && method == "GET")
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
}
