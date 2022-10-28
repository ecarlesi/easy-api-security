using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyApiSecurity.Core
{
    public class JwtSettings
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int Lifetime { get; set; }
    }
}
