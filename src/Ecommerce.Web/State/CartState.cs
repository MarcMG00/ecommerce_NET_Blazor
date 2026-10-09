
using Ecommerce.Shared.ViewModels;
using Ecommerce.Web.ApiClients;

namespace Ecommerce.Web.State;

/// <summary>
/// Current state of the cart during sesion. All are local changes until cart is saved <see cref="SaveAsync"/>
/// If someone needs to be reloaded, has to be subscribed to <see cref="Changed"/>
/// </summary>
public sealed class CartState(ICartApiClient api,
                              CurrentCustomer customer,
                              ToastService toasts,
                              ILogger<CartState> logger)
{
    /// <summary>
    /// List containing each element on the cart
    /// </summary>
    private readonly List<CartItemViewModel> _items = [];

    /// <summary>
    /// Ensure cart has been already loaded
    /// </summary>
    private Task? _loading;

    /// <summary>
    /// Alerts that class has changed => from elements subscribed
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// List containing each element on the cart
    /// </summary>
    public IReadOnlyList<CartItemViewModel> Items => _items;

    /// <summary>
    /// Count number of items in the cart
    /// </summary>
    public int LineCount => _items.Count;

    /// <summary>
    /// Total of the cart
    /// </summary>
    public decimal Total => _items.Sum(item => item.LineTotal);

    /// <summary>
    /// Load cart one time
    /// </summary>
    /// <returns></returns>
    public Task EnsureLoadedAsync() => _loading ??= LoadAsync();

    /// <summary>
    /// Get quantity from item already on the cart (if exists)
    /// </summary>
    /// <param name="articleStoreId"></param>
    /// <returns></returns>
    public int QuantityOf(Guid articleStoreId)
        => _items.FirstOrDefault(item => item.ArticleStoreId == articleStoreId)?.Quantity ?? 0;

    /// <summary>
    /// Add article from ArticleCard component
    /// </summary>
    /// <param name="article"></param>
    /// <param name="store"></param>
    /// <param name="quantity"></param>
    public void Add(ArticleViewModel article, ArticleStoreViewModel store, int quantity)
    {
        if (quantity <= 0 || store.Stock <= 0)
            return;

        // Put new quantity added on the same item from the cart
        var newQuantity = Math.Min(QuantityOf(store.ArticleStoreId) + quantity, store.Stock);

        Upsert(new CartItemViewModel(
            article.Id, article.Name, article.Brand, article.Category,
            store.StoreId, store.StoreName, store.ArticleStoreId,
            store.Price, store.Stock, newQuantity));
    }

    /// <summary>
    /// Update quantity from item cart
    /// </summary>
    /// <param name="articleStoreId"></param>
    /// <param name="quantity"></param>
    public void SetQuantity(Guid articleStoreId, int quantity)
    {
        var index = _items.FindIndex(item => item.ArticleStoreId == articleStoreId);
        // If item not found => return;
        if (index < 0)
            return;

        // if quantity below or at 0, remove item from the cart
        if (quantity <= 0)
            _items.RemoveAt(index);
        // Update quantity from current item cart
        else
            _items[index] = _items[index] with { Quantity = Math.Min(quantity, _items[index].Stock) };

        Changed?.Invoke();
    }

    /// <summary>
    /// Save cart
    /// </summary>
    /// <returns></returns>
    public async Task<bool> SaveAsync()
    {
        try
        {
            // Save all items on the current cart
            await api.SaveCartAsync(customer.Id, [.. _items.Select(item => new CartLineViewModel(item.ArticleStoreId, item.Quantity))]);
            return true;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not save the shopping cart.");
            toasts.Error("The shopping cart could not be saved.");
            return false;
        }
    }

    /// <summary>
    /// Clear cart
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ClearAsync()
    {
        try
        {
            await api.ClearCartAsync(customer.Id);
            _items.Clear();
            Changed?.Invoke();
            return true;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not clear the shopping cart.");
            toasts.Error("The shopping cart could not be cleared.");
            return false;
        }
    }

    /// <summary>
    /// Load cart
    /// </summary>
    /// <returns></returns>
    private async Task LoadAsync()
    {
        try
        {
            var cart = await api.GetCartByCustomerIdAsync(customer.Id);
            _items.Clear();
            _items.AddRange(cart.Items);
            Changed?.Invoke();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Could not load the shopping cart.");
            _loading = null;
        }
    }

    /// <summary>
    /// Add item on the card (add or update)
    /// </summary>
    /// <param name="item"></param>
    private void Upsert(CartItemViewModel item)
    {
        var index = _items.FindIndex(existing => existing.ArticleStoreId == item.ArticleStoreId);

        // If new item => add
        if (index < 0)
            _items.Add(item);
        // Update
        else
            _items[index] = item;

        Changed?.Invoke();
    }
}
