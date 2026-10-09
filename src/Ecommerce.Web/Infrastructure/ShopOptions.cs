using Ecommerce.Shared;

namespace Ecommerce.Web.Infrastructure;

public sealed class ShopOptions
{
    public const string SectionName = "Shop";

    /// <summary>
    /// Fixed customer Id for tests (when login will exists, will get customer by login)
    /// </summary>
    public Guid CustomerId
    {
        get;
        set => field = value != Guid.Empty
            ? value
            : throw new ArgumentException("CustomerId cannot be empty.", nameof(value));
    } = DemoData.CustomerId;
}
