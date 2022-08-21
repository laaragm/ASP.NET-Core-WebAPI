using System;
using System.Net;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace AspNetCore_WebApi_DevIO.Extensions
{
    // Error catching middleware - everything will be logged in elmah
    // We will be able to catch any kind of exception and handle it
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        // All the requests will pass here
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await Next(httpContext);
            }
            catch (Exception exception)
            {
                HandleException(httpContext, exception);
            }
        }

        // Only the exceptions will fall here
        private static void HandleException(HttpContext context, Exception exception)
        {
			exception.Ship(context);
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}