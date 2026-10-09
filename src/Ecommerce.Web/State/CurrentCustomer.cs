using Ecommerce.Web.Infrastructure;
using Microsoft.Extensions.Options;

/// <summary>
/// Current customer for test
/// </summary>
/// <param name="options"></param>
public sealed class CurrentCustomer(IOptions<ShopOptions> options)
{
    public Guid Id { get; } = options.Value.CustomerId;
}
