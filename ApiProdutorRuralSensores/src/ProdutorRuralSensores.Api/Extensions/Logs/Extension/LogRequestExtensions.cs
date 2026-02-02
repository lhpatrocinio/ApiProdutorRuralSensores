using Microsoft.AspNetCore.Mvc;
using ProdutorRuralSensores.Api.Extensions.Logs.Filter;
using ProdutorRuralSensores.Api.Extensions.Logs.Middleware;
using System.Diagnostics.CodeAnalysis;

namespace ProdutorRuralSensores.Api.Extensions.Logs.Extension
{
    [ExcludeFromCodeCoverage]
    public static class LogRequestExtensions
    {
        public static IApplicationBuilder UseSimpleLogRequest(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<LogSimpleRequestMiddleware>();
        }

        public static MvcOptions AddLogRequestFilter(this MvcOptions op)
        {
            op.Filters.Add<LogRequestActionFilter>();

            return op;
        }
    }
}

