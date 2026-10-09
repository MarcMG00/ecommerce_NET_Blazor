namespace Ecommerce.Api.Services;

public sealed class CartService(AppDbContext db) : ICartService
{
    #region GET

    /// <summary>
    /// Get cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<CartViewModel> GetCartByCustomerAsync(Guid customerId, CancellationToken ct)
    {
        await db.EnsureCustomerExistsAsync(customerId, ct);

        var items = await db.ShoppingCartLines
            .AsNoTracking()
            .Where(l => l.ShoppingCart.CustomerId == customerId)
            .OrderBy(l => l.ArticleStore.Article.Name)
            .Select(l => new CartItemViewModel(
                l.ArticleStore.ArticleId,
                l.ArticleStore.Article.Name,
                l.ArticleStore.Article.Brand,
                l.ArticleStore.Article.Category.Name,
                l.ArticleStore.StoreId,
                l.ArticleStore.Store.Name,
                l.ArticleStoreId,
                l.ArticleStore.Price,
                l.ArticleStore.Stock,
                l.Quantity))
            .ToListAsync(ct);

        return new CartViewModel(items);
    }

    #endregion

    #region UPDATE

    /// <summary>
    /// Save cart elements by customer Id
    /// Replace cart with new lines received (update, insert and delete only lines that have changed)
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="RuleViolationException"></exception>
    public async Task SaveCartAsync(Guid customerId, SaveCartViewModel request, CancellationToken ct)
    {
        await db.EnsureCustomerExistsAsync(customerId, ct);

        // Quantities <= 0 are ignored and duplicated lines are summed.
        var requested = request.Lines
            .Where(l => l.Quantity > 0)
            .GroupBy(l => l.ArticleStoreId)
            .ToDictionary(g => g.Key, g => g.Sum(l => l.Quantity));

        var requestedIds = requested.Keys.ToArray();
        var offers = await db.ArticleStores
            .Where(s => requestedIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, ct);

        foreach (var (articleStoreId, quantity) in requested)
        {
            if (!offers.TryGetValue(articleStoreId, out var offer) || !offer.IsActive)
                throw new NotFoundException(nameof(ArticleStore), articleStoreId);

            if (quantity > offer.Stock)
                throw new RuleViolationException(
                    $"Requested quantity ({quantity}) exceeds the available stock ({offer.Stock}) for '{articleStoreId}'.");
        }

        var cart = await db.ShoppingCarts
            .Include(c => c.Lines)
            .SingleOrDefaultAsync(c => c.CustomerId == customerId, ct);

        if (cart is null)
        {
            cart = new ShoppingCart { CustomerId = customerId };
            db.ShoppingCarts.Add(cart);
        }

        foreach (var line in cart.Lines.Where(l => !requested.ContainsKey(l.ArticleStoreId)).ToList())
        {
            db.ShoppingCartLines.Remove(line);
        }

        foreach (var (articleStoreId, quantity) in requested)
        {
            var price = offers[articleStoreId].Price;
            var line = cart.Lines.FirstOrDefault(l => l.ArticleStoreId == articleStoreId);

            if (line is null)
            {
                db.ShoppingCartLines.Add(new ShoppingCartLine
                {
                    ShoppingCartId = cart.Id,
                    ArticleStoreId = articleStoreId,
                    Quantity = quantity,
                    UnitPrice = price,
                });
            }
            else
            {
                line.Quantity = quantity;
                line.UnitPrice = price; // needed the price when saving cart ?
            }
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Clear cart elements by customer Id
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task ClearCartAsync(Guid customerId, CancellationToken ct)
    {
        await db.EnsureCustomerExistsAsync(customerId, ct);

        await db.ShoppingCartLines
            .Where(l => l.ShoppingCart.CustomerId == customerId)
            .ExecuteDeleteAsync(ct);

        await db.ShoppingCarts
            .Where(c => c.CustomerId == customerId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.UpdatedAt, DateTime.UtcNow), ct);
    }

    #endregion
}
