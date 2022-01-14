using AuthorsAndBooks.Components.Utils.Loggers;
using AuthorsAndBooks.Components.Utils.Middlewares;
using AuthorsAndBooks.Utils.Contexts;
using AuthorsAndBooks.Utils.Parsers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorsAndBooks
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AuthorsAndBooksParser>();
            services.AddDbContext<AuthorsAndBooksDbContext>();
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddScoped<FileLogger>();
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Use(async (context, nextMiddleware) =>
            {
                context.Request.EnableBuffering();

                await nextMiddleware.Invoke();
            });
            applicationBuilder.UseRouting();
            applicationBuilder.UseMiddleware<InfoMiddleware>();
            applicationBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}