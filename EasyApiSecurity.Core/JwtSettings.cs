using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EasyApiSecurity.Core
{
    public class JwtSettings
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public byte[] Key { get; set; }
        public KeyType KeyType { get; set; }
        public int Lifetime { get; set; }
    }

    public enum KeyType
    {
        Certificate, SymmetricKey
    }
}
