namespace Ecommerce.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEcommerceApi(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Ecommerce")));

            // Services de negocio: los controllers dependen de la interfaz, no de la implementación.
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IFavouriteService, FavouriteService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<DataSeeder>();

            services.AddProblemDetails();
            services.AddExceptionHandler<ApiExceptionHandler>();
            services.AddOpenApi();

            return services;
        }
    }

    extension(WebApplication app)
    {
        /// <summary>Crea el esquema si no existe y ejecuta el DataSeeder (sólo si Database:AutoInitialize = true).</summary>
        public async Task InitialiseDatabaseAsync()
        {
            await using var scope = app.Services.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();

            await scope.ServiceProvider.GetRequiredService<DataSeeder>().SeedAsync();
        }
    }
}
