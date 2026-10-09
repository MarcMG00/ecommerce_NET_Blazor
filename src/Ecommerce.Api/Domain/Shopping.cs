namespace Ecommerce.Api.Domain;

[Table("shopping_carts")]
[Index(nameof(CustomerId), IsUnique = true)]
public class ShoppingCart : Entity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<ShoppingCartLine> Lines { get; set; } = [];
}

[Table("shopping_cart_lines")]
[Index(nameof(ShoppingCartId), nameof(ArticleStoreId), IsUnique = true)]
public class ShoppingCartLine : Entity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Guid ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; } = null!;

    public Guid ArticleStoreId { get; set; }
    public ArticleStore ArticleStore { get; set; } = null!;
}

[Table("purchase_lists")]
public class PurchaseList : Entity
{
    [MaxLength(150)] public required string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<PurchaseListElement> Elements { get; set; } = [];
}

[Table("purchase_list_elements")]
public class PurchaseListElement : Entity
{
    public PurchaseListElementType Type { get; set; }
    public int Quantity { get; set; }

    public Guid PurchaseListId { get; set; }
    public PurchaseList PurchaseList { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? ArticleId { get; set; }
    public Article? Article { get; set; }
}
