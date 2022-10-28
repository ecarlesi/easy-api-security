using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Net.Mime;
using System.Security;

namespace EasyApiSecurity.Core
{
    public class Middleware
    {
        private readonly RequestDelegate next;
        private MiddlewareContext context;
        private JwtProvider jwt;

        public Middleware(RequestDelegate next, IConfiguration configuration, IOptions<MiddlewareContext> options)
        {
            this.next = next;
            this.context = options.Value;
            this.jwt = JwtProvider.Create(this.context.JwtSettings);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string method = context.Request.Method;
            string path = context.Request.Path;
            string token = context.Request.GetJwtToken();

            JwtInformations.Current = new JwtInformations();

            if (!String.IsNullOrWhiteSpace(token))
            {
                JwtInformations.Current = this.jwt.GetInformations(token);
            }

            bool canAccess = this.context.Storage.CanAccess(JwtInformations.Current, path, method);

            if (!canAccess)
            {
                throw new SecurityException();
            }

            await this.next(context);
        }
    }
}
