using Microsoft.OpenApi.Models;

namespace PDMS.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerModule(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "PDMS API", Version = "0.0.1" });

                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "Jhipster.xml");
                //c.IncludeXmlComments(filePath);

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                /*{
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,

                        },
                        new List<string>()
                    }
                });*/
                {
                    { securityScheme, new[] { "Bearer" } }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseApplicationSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "{documentName}/api-docs";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/v2/api-docs", "PDMS API");
            });
            return app;
        }
    }
}
