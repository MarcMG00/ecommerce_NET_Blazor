using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController(ICartService cartService) : ControllerBase
{
    #region GET

    /// <summary>
    /// Get cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{customerId:guid}")]
    public async Task<CartViewModel> GetCartByCustomerAsync(Guid customerId, CancellationToken ct)
        => await cartService.GetCartByCustomerAsync(customerId, ct);

    #endregion

    #region UPDATE

    /// <summary>
    /// Save cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPut("{customerId:guid}")]
    public async Task SaveCartAsync(Guid customerId, SaveCartViewModel request, CancellationToken ct)
    {
        await cartService.SaveCartAsync(customerId, request, ct);
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Clear cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpDelete("{customerId:guid}")]
    public async Task ClearCartAsync(Guid customerId, CancellationToken ct)
    {
        await cartService.ClearCartAsync(customerId, ct);
    }

    #endregion
}
