using Ecommerce.Shared.ViewModels;

namespace Ecommerce.Web.ApiClients;

public sealed class ArticlesApiClient(HttpClient http) : IArticlesApiClient
{
    private readonly string apiController = "api/articles";
    
    #region GET

    /// <summary>
    /// Get all catalog => all active articles with theyre active stores, promotions, etc.
    /// </summary>
    public async Task<IReadOnlyList<ArticleViewModel>> GetAllArticlesAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<List<ArticleViewModel>>($"{apiController}", ct) ?? [];

    #endregion
}
