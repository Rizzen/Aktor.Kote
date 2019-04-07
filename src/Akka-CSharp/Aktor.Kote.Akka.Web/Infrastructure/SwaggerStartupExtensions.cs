using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Aktor.Kote.Akka.Web.Infrastructure
{
    internal static class SwaggerStartupExtensions
    {
        public static IServiceCollection RegisterSwaggerGenerator(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.AddSwaggerGen(options =>
            {
                options.DocInclusionPredicate((version, apiDescription) =>
                {
                    var routePrefix = configuration["ROUTE_PREFIX"];
                    if (!string.IsNullOrEmpty(routePrefix)
                        && !apiDescription.RelativePath.StartsWith(routePrefix))
                    {
                        return false;
                    }

                    return true;
                });

                options.DescribeAllEnumsAsStrings();

                options.SwaggerDoc(
                    "v1",
                    new Info
                    {
                        Version = "v1",
                        Title = "Akka.Kote"
                    });
            });
        }
        
        public static IApplicationBuilder UseSwaggerAndReDoc(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger(setupOptions =>
            {
                var swaggerPrefix = configuration["ROUTE_PREFIX"] + "/" + "swagger";
                setupOptions.RouteTemplate = $"{swaggerPrefix}/{{documentName}}/swagger.json";
            });
            
            app.UseSwaggerUI(swaggerOptions =>
            {
                var swaggerPrefix = configuration["ROUTE_PREFIX"] + "/" + "swagger";
                swaggerOptions.SwaggerEndpoint($"./v1/swagger.json", "Kote API V1");
                swaggerOptions.RoutePrefix = swaggerPrefix;
                swaggerOptions.DocExpansion(DocExpansion.None);
                swaggerOptions.DefaultModelRendering(ModelRendering.Model);
                swaggerOptions.DocumentTitle = "Kote API";
            });
            
            return app;
        }
    }
}