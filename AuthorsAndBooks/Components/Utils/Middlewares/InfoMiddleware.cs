using AuthorsAndBooks.Components.Utils.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AuthorsAndBooks.Components.Utils.Middlewares
{
    public class InfoMiddleware
    {
        private readonly RequestDelegate nextMiddleware;

        public InfoMiddleware(RequestDelegate nextMiddleware)
        {
            this.nextMiddleware = nextMiddleware;
        }

        private static bool AreRouteValuesCorrect(HttpContext context)
        {
            RouteValueDictionary routeValues = context.Request.RouteValues;

            return routeValues.ContainsKey("controller") && routeValues.ContainsKey("action");
        }

        private async Task<string> GetMessage(HttpContext context, (DateTime start, DateTime end) requestDateTimes)
        {
            async Task<string> GetBody(Stream bodyStream)
            {
                bodyStream.Seek(0, SeekOrigin.Begin);

                using (StreamReader streamReader = new StreamReader(bodyStream))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }

            HttpRequest request = context.Request;
            string body = await GetBody(request.Body);
            QueryString queryString = context.Request.QueryString;

            return $"{requestDateTimes.start} – Request – Path:{request.Path}; Method:{request.Method}; Body:{(body.Equals(string.Empty) ? "empty" : body)}; " +
                $"Query string:{(queryString.HasValue ? queryString.Value : "none")}; Execution time:{(int)requestDateTimes.end.Subtract(requestDateTimes.start).TotalMilliseconds}ms; " +
                $"Status code:{context.Response.StatusCode}";
        }

        public async Task InvokeAsync(HttpContext context, FileLogger fileLogger)
        {
            if (AreRouteValuesCorrect(context))
            {
                DateTime requestStartDateTime = DateTime.Now;

                await nextMiddleware.Invoke(context);

                fileLogger.LogInformation(await GetMessage(context, (requestStartDateTime, DateTime.Now)));
            }
            else
                await nextMiddleware.Invoke(context);
        }
    }
}