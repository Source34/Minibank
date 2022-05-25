using System.Threading.Tasks;
using Minibank.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Minibank.Web.Models.JwtTokens;
using Minibank.Core.Domains.Messages;

namespace Minibank.Web.Middlewares
{
    public class CustomAuthenticationMiddleware
    {
        public readonly RequestDelegate next;

        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            this.next = next;

        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;

            //Не нашел как скипнуть аутентификация для Swagger
            if (!(path.HasValue && path.Value!.StartsWith("/swagger")))
            {
                if (JwtTokenDuende.TryParse(httpContext.Request.Headers.Authorization, out var jwtToken))
                    jwtToken.EnsureTokenIsNotExpiredOrThrow();
                else
                {
                   throw new CustomAuthenticationException(
                                ErrorMessages.GetAuthenticationErrorMessage(
                                    ErrorMessages.AuthenticationErrorLegend,
                                    ErrorMessages.JwtTokenParsingErrorReason));
                }
            }

            await next(httpContext);
        }
    }
}
