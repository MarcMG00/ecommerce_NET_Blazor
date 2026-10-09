namespace Ecommerce.Web.ApiClients;

public sealed class FavouritesApiClient(HttpClient http) : IFavouritesApiClient
{
    private readonly string apiController = "api/favourites";

    #region GET

    /// <summary>
    /// Get all favourite articles by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Guid>> GetAllFavouritesByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<Guid>>($"{apiController}/{customerId}", ct) ?? [];

    #endregion

    #region UPDATE

    /// <summary>
    /// Put article as favourite (by article Id and customer id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="articleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task AddFavouriteArticle(Guid customerId, Guid articleId, CancellationToken ct = default)
    {
        using var response = await http.PutAsync($"{apiController}/{customerId}/{articleId}", content: null, ct);
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Remove article from favourites (by article Id and customer id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="articleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task RemoveFavouriteArticleAsync(Guid customerId, Guid articleId, CancellationToken ct = default)
    {
        using var response = await http.DeleteAsync($"{apiController}/{customerId}/{articleId}", ct);
        response.EnsureSuccessStatusCode();
    }

    #endregion
}
