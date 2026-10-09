namespace Ecommerce.Api.Services;

public sealed class FavouriteService(AppDbContext db) : IFavouriteService
{
    #region GET

    /// <summary>
    /// Get all favourite articles by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Guid>> GetAllFavouritesByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        await db.EnsureCustomerExistsAsync(customerId, ct);

        return await db.Favourites
            .AsNoTracking()
            .Where(f => f.CustomerId == customerId)
            .Select(f => f.ArticleId)
            .ToListAsync(ct);
    }

    #endregion

    #region UPDATE

    /// <summary>
    /// Put article as favourite (by article Id and customer id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="articleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task AddFavouriteArticle(Guid customerId, Guid articleId, CancellationToken ct)
    {
        if (await db.Favourites.AnyAsync(f => f.CustomerId == customerId && f.ArticleId == articleId, ct))
            return;

        await db.EnsureCustomerExistsAsync(customerId, ct);

        if (!await db.Articles.AnyAsync(a => a.Id == articleId, ct))
            throw new NotFoundException(nameof(Article), articleId);

        db.Favourites.Add(new Favourite { CustomerId = customerId, ArticleId = articleId });
        await db.SaveChangesAsync(ct);
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
    public async Task RemoveFavouriteArticleAsync(Guid customerId, Guid articleId, CancellationToken ct)
    {
        await db.EnsureCustomerExistsAsync(customerId, ct);

        await db.Favourites
            .Where(f => f.CustomerId == customerId && f.ArticleId == articleId)
            .ExecuteDeleteAsync(ct);
    }

    #endregion
}
