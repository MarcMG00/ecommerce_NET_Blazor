using Ecommerce.Shared.Enums;

namespace Ecommerce.Shared.ViewModels;

public sealed record PromotionViewModel(string Name, PromotionType Type, decimal Value);

/// <summary>Precio/stock de un artículo en una tienda concreta (fila de articles_store).</summary>
public sealed record ArticleStoreViewModel(
    Guid StoreId,
    Guid ArticleStoreId,
    string StoreName,
    decimal Price,
    int Stock,
    PromotionViewModel? Promotion);

public sealed record ArticleViewModel(
    Guid Id,
    string Name,
    string? Description,
    string? Brand,
    string Category,
    List<ArticleStoreViewModel> Stores);
