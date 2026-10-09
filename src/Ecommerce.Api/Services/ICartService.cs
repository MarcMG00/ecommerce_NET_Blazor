namespace Ecommerce.Api.Services;

public interface ICartService
{
    #region GET

    /// <summary>
    /// Get cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<CartViewModel> GetCartByCustomerAsync(Guid customerId, CancellationToken ct);

    #endregion

    #region UPDATE

    /// <summary>
    /// Save cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task SaveCartAsync(Guid customerId, SaveCartViewModel request, CancellationToken ct);

    #endregion

    #region DELETE

    /// <summary>
    /// Clear cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task ClearCartAsync(Guid customerId, CancellationToken ct);

    #endregion
}
