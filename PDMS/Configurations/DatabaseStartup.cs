using Microsoft.EntityFrameworkCore;

namespace PDMS.Configurations
{
    public static class DatabaseStartup
    {
        public static IServiceCollection AddDatabaseModule<T>(this IServiceCollection services, IConfiguration configuration)
            where T : DbContext
        {
           
            services.AddDbContext<T>(context =>
            {
                context.UseSqlServer(configuration.GetConnectionString("PdmsDbContext"));
            });
            services.AddScoped<DbContext>(provider => provider.GetService<T>() ?? throw new ArgumentNullException(nameof(T)));

            return services;

        }

        public static IApplicationBuilder UseApplicationDatabase<T>(this IApplicationBuilder app,
            IServiceProvider serviceProvider, IHostEnvironment environment)
            where T : DbContext
        {
            if (environment.IsDevelopment() || environment.IsProduction())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<T>();
                    context.Database.OpenConnection();
                    context.Database.Migrate();
                    context.Database.EnsureCreated();
                }
            }
            return app;
        }
    }
}
