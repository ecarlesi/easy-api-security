using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyApiSecurity.Core
{
    public interface IAuthorizationManager
    {
        bool CanAccess(JwtInformations informations, string resource, string method);
    }
}
