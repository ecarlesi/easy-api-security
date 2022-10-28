using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Net;
using System;
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

            bool canAccess = this.context.AuthorizationManager.CanAccess(JwtInformations.Current, path, method);

            try
            {
                if (!canAccess)
                {
                    throw new SecurityException();
                }

                await this.next(context);
            }
            catch (SecurityException e)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
            catch (Exception e)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(e.Message); //TODO avoid to show the internal error to the client
            }
        }
    }
}
