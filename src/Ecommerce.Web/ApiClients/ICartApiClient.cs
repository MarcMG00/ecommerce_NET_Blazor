using Ecommerce.Shared.ViewModels;

namespace Ecommerce.Web.ApiClients;

public interface ICartApiClient
{
    #region GET

    /// <summary>
    /// Get cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<CartViewModel> GetCartByCustomerIdAsync(Guid customerId, CancellationToken ct = default);

    #endregion

    #region UPDATE

    /// <summary>
    /// Save cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="lines"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task SaveCartAsync(Guid customerId, IReadOnlyList<CartLineViewModel> lines, CancellationToken ct = default);

    #endregion

    #region DELETE

    /// <summary>
    /// Clear cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task ClearCartAsync(Guid customerId, CancellationToken ct = default);

    #endregion
}
