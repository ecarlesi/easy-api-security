using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace EasyApiSecurity.Core
{
    public class JwtProvider
    {
        private const int DefaultTokenLifetimeInMinutes = 120;

        private readonly JwtSettings? _settings;

        private JwtProvider(JwtSettings? settings)
        {
            if (settings == null)
            {
                throw new ArgumentException("settings is null");
            }

            if (string.IsNullOrWhiteSpace(settings.Issuer))
            {
                throw new ArgumentException("Issuer is empty");
            }

            if (string.IsNullOrWhiteSpace(settings.Audience))
            {
                throw new ArgumentException("Audience is empty");
            }

            if (settings.Key == null || settings.Key.Length == 0)
            {
                throw new ArgumentException("Invalid key");
            }

            _settings = settings;
        }

        private static JwtProvider? _instance;
        private static readonly object Lock = new object();

        public static JwtProvider Create(JwtSettings? settings)
        {
            if (_instance != null) return _instance;
            
            lock (Lock)
            {
                _instance ??= new JwtProvider(settings);
            }

            return _instance;
        }

        public static JwtProvider Instance()
        {
            if (_instance == null)
            {
                throw new JwtProviderNotInitializedException();
            }

            return _instance;
        }

        public string CreateToken(JwtInformations informations)
        {
            if (informations == null)
            {
                throw new InvalidInformationException();
            }

            DateTime expiration = DateTime.UtcNow.AddMinutes(_settings!.Lifetime == 0 ? DefaultTokenLifetimeInMinutes : this._settings.Lifetime);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim("name", informations.Name!),
                     new Claim("email", informations.Email!),
                     new Claim("roles", JsonHelper.Serialize(informations.Roles!)),
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = expiration,
                SigningCredentials = new SigningCredentials(this.GetSecurityKey(), SecurityAlgorithms.HmacSha256Signature),
                Issuer = this._settings.Issuer,
                Audience = this._settings.Audience
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public JwtInformations? GetInformations(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings?.Issuer,
                    ValidAudience = _settings?.Audience, 
                    IssuerSigningKey = this.GetSecurityKey(),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityToken jsonToken = handler.ReadToken(token);
                JwtSecurityToken? tokenS = jsonToken as JwtSecurityToken;

                JwtInformations? informations = new JwtInformations
                {
                    Name = GetClaimValue(tokenS, "name"),
                    Email = GetClaimValue(tokenS, "email")
                };

                string json = GetClaimValue(tokenS, "roles");

                informations.Roles = JsonHelper.Deserialize<string[]>(json);

                return informations;
            }
            catch (Exception)
            {
                throw new SecurityException();
            }
        }

        public void Validate(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings?.Issuer,
                ValidAudience = _settings?.Audience,
                IssuerSigningKey = this.GetSecurityKey(),
                ValidateIssuer = true, 
                ValidateAudience = true, 
                ClockSkew = TimeSpan.Zero
            }, out _);
        }

        private SecurityKey GetSecurityKey()
        {
            return _settings?.KeyType switch
            {
                KeyType.SymmetricKey => new SymmetricSecurityKey(this._settings.Key),
                KeyType.Certificate => new X509SecurityKey(new X509Certificate2(this._settings.Key)),
                _ => throw new InvalidDataException()
            };
        }

        private static string GetClaimValue(JwtSecurityToken? token, string claimName)
        {
            Claim? c = token?.Claims.FirstOrDefault(x => x.Type == claimName);

            return c == null ? "" : c.Value;
        }
    }
}
