namespace Ecommerce.Api.Services;

public interface IFavouriteService
{
    #region GET

    /// <summary>
    /// Get all favourite articles by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<IReadOnlyList<Guid>> GetAllFavouritesByCustomerIdAsync(Guid customerId, CancellationToken ct);

    #endregion

    #region UPDATE

    /// <summary>
    /// Put article as favourite (by article Id and customer id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="articleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task AddFavouriteArticle(Guid customerId, Guid articleId, CancellationToken ct);

    #endregion

    #region DELETE

    /// <summary>
    /// Remove article from favourites (by article Id and customer id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="articleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RemoveFavouriteArticleAsync(Guid customerId, Guid articleId, CancellationToken ct);

    #endregion
}
