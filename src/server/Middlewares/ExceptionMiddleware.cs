using System;
using System.Threading.Tasks;
using Server.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Server.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;


        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UserDataException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("400 Invalid input");
            }
            catch (BusinessLogicException)
            {
                context.Response.StatusCode = 436;
                await context.Response.WriteAsync("436 Invalid data");
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("500 Server error");
            }
        }
    }

}