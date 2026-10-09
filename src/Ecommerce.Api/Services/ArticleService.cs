namespace Ecommerce.Api.Services;

public sealed class ArticleService(AppDbContext db) : IArticleService
{
    #region GET

    /// <summary>
    /// Get all catalog => all active articles with theyre active stores, promotions, etc.
    /// </summary>
    public async Task<IReadOnlyList<ArticleViewModel>> GetAllArticlesAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await db.Articles
            .AsNoTracking()
            .Where(a => a.IsActive && a.Stores.Any(s => s.IsActive))
            .OrderBy(a => a.Name)
            .Select(a => new ArticleViewModel(
                a.Id,
                a.Name,
                a.Description,
                a.Brand,
                a.Category.Name,
                a.Stores
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Store.Name)
                    .Select(s => new ArticleStoreViewModel(
                        s.StoreId,
                        s.Id,
                        s.Store.Name,
                        s.Price,
                        s.Stock,
                        s.Promotions
                            .Where(p => p.Promotion.StartDate <= now && p.Promotion.EndDate >= now)
                            .Select(p => new PromotionViewModel(p.Promotion.Name, p.Promotion.Type, p.Promotion.Value))
                            .FirstOrDefault()))
                    .ToList()))
            .ToListAsync(ct);
    }

    #endregion
}
