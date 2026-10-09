namespace Ecommerce.Api.Domain;

[Table("categories")]
[Index(nameof(Name), IsUnique = true)]
public class Category : Entity
{
    [MaxLength(100)] public required string Name { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

[Table("stores")]
public class Store : Entity
{
    [MaxLength(150)] public required string Name { get; set; }
    [MaxLength(250)] public string? Address { get; set; }
    [MaxLength(100)] public string? City { get; set; }
    [MaxLength(20)] public string? ZipCode { get; set; }
    [MaxLength(30)] public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}

[Table("articles")]
[Index(nameof(BarCode), IsUnique = true)]
public class Article : Entity
{
    [MaxLength(200)] public required string Name { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    [MaxLength(100)] public string? Brand { get; set; }
    [MaxLength(100)] public string? BarCode { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<ArticleImage> Images { get; set; } = [];
    public ICollection<ArticleStore> Stores { get; set; } = [];
}

[Table("article_images")]
public class ArticleImage : Entity
{
    [MaxLength(1000)] public required string Url { get; set; }
    public int SortOrder { get; set; }
    public bool IsMain { get; set; }

    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
}

/// <summary>Precio y stock de un artículo en una tienda.</summary>
[Table("articles_store")]
[Index(nameof(ArticleId), nameof(StoreId), IsUnique = true)]
public class ArticleStore : Entity
{
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public ICollection<ArticlePromotion> Promotions { get; set; } = [];
}

[Table("promotions")]
public class Promotion : Entity
{
    [MaxLength(150)] public required string Name { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Value { get; set; }
    public PromotionType Type { get; set; }
}

[Table("article_promotions")]
[Index(nameof(ArticleStoreId), nameof(PromotionId), IsUnique = true)]
public class ArticlePromotion : Entity
{
    public Guid ArticleStoreId { get; set; }
    public ArticleStore ArticleStore { get; set; } = null!;

    public Guid PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;
}
