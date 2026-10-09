using Ecommerce.Web.ApiClients;
using Ecommerce.Web.State;

namespace Ecommerce.Web.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddShop(IConfiguration configuration)
        {
            var apiBaseUrl = configuration["Api:BaseUrl"]
                ?? throw new InvalidOperationException("Api:BaseUrl is not configured.");

            services.Configure<ShopOptions>(configuration.GetSection(ShopOptions.SectionName));

            // Un ApiClient por controller de la API, todos con la misma dirección base.
            Action<HttpClient> configureClient = client => client.BaseAddress = new Uri(apiBaseUrl);
            services.AddHttpClient<IArticlesApiClient, ArticlesApiClient>(configureClient);
            services.AddHttpClient<IFavouritesApiClient, FavouritesApiClient>(configureClient);
            services.AddHttpClient<ICartApiClient, CartApiClient>(configureClient);

            // Un circuito de Blazor = una sesión de usuario: el estado vive mientras dure.
            services.AddScoped<CurrentCustomer>();
            services.AddScoped<ToastService>();
            services.AddScoped<FavouritesState>();
            services.AddScoped<CartState>();

            return services;
        }
    }
}
