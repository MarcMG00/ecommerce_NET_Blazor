using Ecommerce.Shared.ViewModels;

namespace Ecommerce.Web.ApiClients;

public sealed class CartApiClient(HttpClient http) : ICartApiClient
{
    private readonly string apiController = "api/cart";

    #region GET

    /// <summary>
    /// Get cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CartViewModel> GetCartByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await http.GetFromJsonAsync<CartViewModel>($"{apiController}/{customerId}", ct) ?? new CartViewModel([]);

    #endregion

    #region UPDATE

    /// <summary>
    /// Save cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="lines"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task SaveCartAsync(Guid customerId, IReadOnlyList<CartLineViewModel> lines, CancellationToken ct = default)
    {
        using var response = await http.PutAsJsonAsync($"{apiController}/{customerId}", new SaveCartViewModel(lines), ct);
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Clear cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task ClearCartAsync(Guid customerId, CancellationToken ct = default)
    {
        using var response = await http.DeleteAsync($"{apiController}/{customerId}", ct);
        response.EnsureSuccessStatusCode();
    }

    #endregion
}
