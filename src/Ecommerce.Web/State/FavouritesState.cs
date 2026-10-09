namespace Ecommerce.Web.State;
using Ecommerce.Web.ApiClients;

public sealed class FavouritesState(IFavouritesApiClient api,
                                    CurrentCustomer customer,
                                    ToastService toasts,
                                    ILogger<FavouritesState> logger)
{
    /// <summary>
    /// List of unique article Ids
    /// </summary>
    private readonly HashSet<Guid> _articleIds = [];

    /// <summary>
    /// Stock Task completed => get all favourite articles for first render
    /// </summary>
    private Task? _loading;

    /// <summary>
    /// Alerts that class has changed => from elements subscribed
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Checks if article is from favourites (by article Id)
    /// </summary>
    /// <param name="articleId"></param>
    /// <returns></returns>
    public bool IsFavourite(Guid articleId) => _articleIds.Contains(articleId);

    /// <summary>
    /// Get favourite articles if Task not already completed
    /// </summary>
    /// <returns></returns>
    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    /// <summary>
    /// Add/ Remove article from favourites
    /// </summary>
    /// <param name="articleId"></param>
    /// <returns></returns>
    public async Task ToggleAsync(Guid articleId)
    {
        var adding = !_articleIds.Contains(articleId);

        try
        {
            if (adding)
            {
                // Save on DB
                await api.AddFavouriteArticle(customer.Id, articleId);
                _articleIds.Add(articleId);
            }
            else
            {
                // Remove on DB
                await api.RemoveFavouriteArticleAsync(customer.Id, articleId);
                _articleIds.Remove(articleId);
            }

            Changed?.Invoke();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not update favourite {ArticleId}.", articleId);
            toasts.Error("The favourite could not be updated.");
        }
    }

    /// <summary>
    ///  Task to get favourtite articles
    /// </summary>
    /// <returns></returns>
    public async Task LoadAsync()
    {
        try
        {
            var ids = await api.GetAllFavouritesByCustomerIdAsync(customer.Id);
            _articleIds.Clear();
            _articleIds.UnionWith(ids);
            Changed?.Invoke();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not load the favourites.");
            _loading = null;
        }
    }
}
