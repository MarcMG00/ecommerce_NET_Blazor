using Ecommerce.Shared.ViewModels;

namespace Ecommerce.Web.ApiClients;

public interface IArticlesApiClient
{
    #region GET

    /// <summary>
    /// Get all catalog => all active articles with theyre active stores, promotions, etc.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ArticleViewModel>> GetAllArticlesAsync(CancellationToken ct = default);

    #endregion
}
