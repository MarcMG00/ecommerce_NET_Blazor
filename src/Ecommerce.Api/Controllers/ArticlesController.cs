using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController(IArticleService articleService) : ControllerBase
{
    #region GET

    /// <summary>
    /// Get all catalog => all active articles with theyre active stores, promotions, etc.
    /// </summary>
    [HttpGet]
    public async Task<IReadOnlyList<ArticleViewModel>> GetAllArticlesAsync(CancellationToken ct) => await articleService.GetAllArticlesAsync(ct);

    #endregion
}
