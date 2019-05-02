using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace OrleansKat.Api.Infrastructure
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
                    return string.IsNullOrEmpty(routePrefix) 
                           || apiDescription.RelativePath.StartsWith(routePrefix);
                });

                options.DescribeAllEnumsAsStrings();
                
                options.SwaggerDoc(
                    "v1",
                    new Info
                    {
                        Version = "v1",
                        Title = "Kote API",
                        Description = "Kote"
                    });
                
                options.SetCommentsPath();
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
        
        private static void SetCommentsPath(this SwaggerGenOptions options)
        {
            var basePath = AppContext.BaseDirectory;
            var xmlPath = Path.Combine(basePath, "OrleansKat.Api.xml");
            // var xmlPathModel = Path.Combine(basePath, "Atlas.TrackAndTrace.Models.xml");
            options.IncludeXmlComments(xmlPath);
            // options.IncludeXmlComments(xmlPathModel);
        }
    }
}