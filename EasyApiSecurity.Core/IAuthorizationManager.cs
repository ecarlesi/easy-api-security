namespace EasyApiSecurity.Core
{
    public interface IAuthorizationManager
    {
        bool CanAccess(JwtInformations? informations, string resource, string method);
    }
}
