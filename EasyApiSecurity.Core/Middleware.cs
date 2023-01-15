using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security;

namespace EasyApiSecurity.Core
{
    public class Middleware
    {
        private readonly RequestDelegate _next;
        private readonly MiddlewareContext _context;
        private readonly JwtProvider _jwt;

        public Middleware(RequestDelegate next, IOptions<MiddlewareContext> options)
        {
            _next = next;
           _context = options.Value;
            _jwt = JwtProvider.Create(this._context.JwtSettings);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string method = context.Request.Method;
                string path = context.Request.Path;
                string token = context.Request.GetJwtToken();

                JwtInformations.Current = new JwtInformations();

                if (!string.IsNullOrWhiteSpace(token))
                {
                    JwtInformations.Current = this._jwt.GetInformations(token);
                }

                bool canAccess = this._context.AuthorizationManager.CanAccess(JwtInformations.Current, path, method);

                if (!canAccess)
                {
                    throw new SecurityException();
                }

                await this._next(context);
            }
            catch (SecurityException)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
            catch (Exception e)
            {
                switch (_context.ErrorHandlerBehavior)
                {
                    case MiddlewareErrorHandlerBehavior.Throw:
                        throw;
                    case MiddlewareErrorHandlerBehavior.ShowMessage:
                    case MiddlewareErrorHandlerBehavior.ShowGeneric:
                    default:
                    {
                        string message = this._context.ErrorHandlerBehavior == MiddlewareErrorHandlerBehavior.ShowMessage ? e.Message : "Something bad happened :(";

                        context.Response.Clear();
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync(message);
                        break;
                    }
                }
            }
        }
    }
}
