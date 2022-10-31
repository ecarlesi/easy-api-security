using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security;

namespace EasyApiSecurity.Core
{
    public class Middleware
    {
        private readonly RequestDelegate next;
        private MiddlewareContext context;
        private JwtProvider jwt;

        public Middleware(RequestDelegate next, IOptions<MiddlewareContext> options)
        {
            this.next = next;
            this.context = options.Value;
            this.jwt = JwtProvider.Create(this.context.JwtSettings);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string method = context.Request.Method;
                string path = context.Request.Path;
                string token = context.Request.GetJwtToken();

                JwtInformations.Current = new JwtInformations();

                if (!String.IsNullOrWhiteSpace(token))
                {
                    JwtInformations.Current = this.jwt.GetInformations(token);
                }

                bool canAccess = this.context.AuthorizationManager.CanAccess(JwtInformations.Current, path, method);

                if (!canAccess)
                {
                    throw new SecurityException();
                }
                else
                {
                    await this.next(context);
                }
            }
            catch (SecurityException e)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
            catch (Exception e)
            {
                if (this.context.ErrorHandlerBehavior == MiddlewareErrorHandlerBehavior.Throw)
                {
                    throw;
                }
                else
                {
                    string message = this.context.ErrorHandlerBehavior == MiddlewareErrorHandlerBehavior.ShowMessage ? e.Message : "Something bad happened :(";

                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync(message);

                }
            }
        }
    }
}
