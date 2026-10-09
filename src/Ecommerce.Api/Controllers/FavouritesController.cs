using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/favourites")]
public class FavouritesController(IFavouriteService favouriteService) : ControllerBase
{
    #region GET

    /// <summary>
    /// Get all favourite articles by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{customerId:guid}")]
    public async Task<IReadOnlyList<Guid>> GetAllFavouritesByCustomerIdAsync(Guid customerId, CancellationToken ct)
        => await favouriteService.GetAllFavouritesByCustomerIdAsync(customerId, ct);

    #endregion

    #region UPDATE

    /// <summary>
    /// Put article as favourite (by article Id and customer id)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="articleId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPut("{customerId:guid}/{articleId:guid}")]
    public async Task AddFavouriteArticle(Guid customerId, Guid articleId, CancellationToken ct)
    {
        await favouriteService.AddFavouriteArticle(customerId, articleId, ct);
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
    [HttpDelete("{customerId:guid}/{articleId:guid}")]
    public async Task RemoveFavouriteArticleAsync(Guid customerId, Guid articleId, CancellationToken ct)
    {
        await favouriteService.RemoveFavouriteArticleAsync(customerId, articleId, ct);
    }

    #endregion
}
