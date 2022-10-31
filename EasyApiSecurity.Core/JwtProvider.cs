using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EasyApiSecurity.Core
{
    public class JwtProvider
    {
        private static readonly int DEFAULT_TOKEN_LIFETIME_IN_MINUTES = 120;

        private JwtSettings settings;

        private JwtProvider()
        { 
        }

        private JwtProvider(JwtSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentException("settings is null");
            }

            if (String.IsNullOrWhiteSpace(settings.Issuer))
            {
                throw new ArgumentException("Issuer is empty");
            }

            if (String.IsNullOrWhiteSpace(settings.Audience))
            {
                throw new ArgumentException("Audience is empty");
            }

            if (settings.Key == null || settings.Key.Length == 0)
            {
                throw new ArgumentException("Invalid key");
            }

            this.settings = settings;
        }

        private static JwtProvider instance;
        private static object LOCK = new object();

        public static JwtProvider Create(JwtSettings settings)
        {
            if (instance == null)
            {
                lock (LOCK)
                {
                    if (instance == null)
                    {
                        instance = new JwtProvider(settings);
                    }
                }
            }

            return instance;
        }

        public static JwtProvider Instance()
        {
            if (instance == null)
            {
                throw new JwtProviderNotInizializedException();
            }

            return instance;
        }

        public string CreateToken(JwtInformations informations)
        {
            if (informations == null)
            {
                throw new InvalidInformationException();
            }

            DateTime expiration = DateTime.UtcNow.AddMinutes(this.settings.Lifetime == 0 ? DEFAULT_TOKEN_LIFETIME_IN_MINUTES : this.settings.Lifetime);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                     new Claim("name", informations.Name),
                     new Claim("email", informations.Email),
                     new Claim("roles", JsonHelper.Serialize(informations.Roles)),
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = expiration,
                SigningCredentials = new SigningCredentials(this.GetSecurityKey(), SecurityAlgorithms.HmacSha256Signature),
                Issuer = this.settings.Issuer,
                Audience = this.settings.Audience
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public JwtInformations GetInformations(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = this.settings.Issuer,
                    ValidAudience = this.settings.Audience, 
                    IssuerSigningKey = this.GetSecurityKey(),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityToken jsonToken = handler.ReadToken(token);
                JwtSecurityToken tokenS = jsonToken as JwtSecurityToken;

                JwtInformations informations = new JwtInformations();

                informations.Name = GetClaimValue(tokenS, "name");
                informations.Email = GetClaimValue(tokenS, "email");

                string json = GetClaimValue(tokenS, "roles");

                informations.Roles = JsonHelper.Deserialize<string[]>(json);

                return informations;
            }
            catch (Exception e)
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
                ValidIssuer = this.settings.Issuer,
                ValidAudience = this.settings.Audience,
                IssuerSigningKey = this.GetSecurityKey(),
                ValidateIssuer = true, 
                ValidateAudience = true, 
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
        }

        private SecurityKey GetSecurityKey()
        {
            if (settings.KeyType == KeyType.SymmetricKey)
            {
                return new SymmetricSecurityKey(this.settings.Key);
            }
            else if (settings.KeyType == KeyType.Certificate)
            {
                return new X509SecurityKey(new X509Certificate2(this.settings.Key));
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        private static string GetClaimValue(JwtSecurityToken token, string claimName)
        {
            Claim c = token.Claims.Where(x => x.Type == claimName).FirstOrDefault();

            if (c == null)
            {
                return "";
            }

            return c.Value;
        }
    }
}
