using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace EasyApiSecurity.Core
{
    public static class Extensions
    {
        public static IApplicationBuilder UseEas(this IApplicationBuilder builder, MiddlewareContext middlewareConfiguration)
        {
            JwtProvider.Create(middlewareConfiguration.JwtSettings);

            return builder.UseMiddleware<Middleware>(Options.Create(middlewareConfiguration));
        }

        public static string GetJwtToken(this HttpRequest? request)
        {
            if (request?.Headers == null)
            {
                return string.Empty;
            }

            if (!request.Headers.ContainsKey("Authorization"))
            {
                return string.Empty;
            }

            string authorization = request.Headers["Authorization"].ToString();

            return string.IsNullOrEmpty(authorization) ? string.Empty : authorization.Split(' ').Last();
        }
    }
}
