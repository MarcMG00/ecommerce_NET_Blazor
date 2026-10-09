namespace Ecommerce.Api.Services;

public interface IArticleService
{
    #region GET

    /// <summary>
    /// Get all catalog => all active articles with theyre active stores, promotions, etc.
    /// </summary>
    Task<IReadOnlyList<ArticleViewModel>> GetAllArticlesAsync(CancellationToken ct);

    #endregion
}
