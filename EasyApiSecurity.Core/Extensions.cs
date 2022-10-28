using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyApiSecurity.Core
{
    public static class Extensions
    {
        public static IApplicationBuilder UseEas(this IApplicationBuilder builder, MiddlewareContext middlewareConfiguration)
        {
            JwtProvider.Create(middlewareConfiguration.JwtSettings);

            return builder.UseMiddleware<Middleware>(Options.Create(middlewareConfiguration));
        }

        public static string GetJwtToken(this HttpRequest request)
        {
            if (request == null)
            {
                return String.Empty;
            }

            if (!request.Headers.ContainsKey("Authorization"))
            {
                return String.Empty;
            }

            string authorization = request.Headers["Authorization"].ToString();

            if (String.IsNullOrEmpty(authorization))
            {
                return String.Empty;
            }

            return authorization.Split(' ').Last();
        }
    }
}
