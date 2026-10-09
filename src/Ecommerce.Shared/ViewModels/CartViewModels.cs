using System.Text.Json.Serialization;

namespace Ecommerce.Shared.ViewModels;

/// <summary>
/// Vm for info items on cart
/// </summary>
/// <param name="ArticleId"></param>
/// <param name="ArticleName"></param>
/// <param name="ArticleBrand"></param>
/// <param name="ArticleCategory"></param>
/// <param name="StoreId"></param>
/// <param name="StoreName"></param>
/// <param name="ArticleStoreId"></param>
/// <param name="UnitPrice"></param>
/// <param name="Stock"></param>
/// <param name="Quantity"></param>
public sealed record CartItemViewModel(
    Guid ArticleId,
    string ArticleName,
    string? ArticleBrand,
    string ArticleCategory,
    Guid StoreId,
    string StoreName,
    Guid ArticleStoreId,
    decimal UnitPrice,
    int Stock,
    int Quantity)
{
    [JsonIgnore]
    public decimal LineTotal => UnitPrice * Quantity;
}

/// <summary>
/// The main cart Vm (with all the items)
/// </summary>
/// <param name="Items"></param>
public sealed record CartViewModel(IReadOnlyList<CartItemViewModel> Items);

/// <summary>
/// Stock elements when saving cart to a new CartLineViewModel
/// </summary>
/// <param name="ArticleStoreId"></param>
/// <param name="Quantity"></param>
public sealed record CartLineViewModel(Guid ArticleStoreId, int Quantity);

/// <summary>
/// Replace all the content from the cart
/// </summary>
/// <param name="Lines"></param>
public sealed record SaveCartViewModel(IReadOnlyList<CartLineViewModel> Lines);
