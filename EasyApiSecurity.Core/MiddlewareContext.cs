using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyApiSecurity.Core
{
    public delegate void MiddlewareErrorHandler(string path, string method, Exception e);

    public class MiddlewareContext
    {
        public IAuthorizationManager AuthorizationManager { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public MiddlewareErrorHandlerBehavior ErrorHandlerBehavior { get; set; }
    }
}
