namespace Ecommerce.Api.Domain;

[Table("users")]
[Index(nameof(Email), IsUnique = true)]
public class User : Entity
{
    [MaxLength(150)] public required string Email { get; set; }
    [MaxLength(255)] public required string PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public Role Role { get; set; }
}

[Table("customers")]
[Index(nameof(UserId), IsUnique = true)]
public class Customer : Entity
{
    [MaxLength(100)] public required string FirstName { get; set; }
    [MaxLength(100)] public required string LastName { get; set; }
    [MaxLength(30)] public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    /// <summary>Tienda de referencia del cliente.</summary>
    public Guid? StoreReferenceId { get; set; }
    public Store? StoreReference { get; set; }
}

[Table("vendors")]
[Index(nameof(UserId), IsUnique = true)]
public class Vendor : Entity
{
    [MaxLength(100)] public required string FirstName { get; set; }
    [MaxLength(100)] public required string LastName { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
}

[Table("order_preparers")]
[Index(nameof(UserId), IsUnique = true)]
public class OrderPreparer : Entity
{
    [MaxLength(100)] public required string FirstName { get; set; }
    [MaxLength(100)] public required string LastName { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

[Table("customer_stores")]
[Index(nameof(CustomerId), nameof(StoreId), IsUnique = true)]
public class CustomerStore : Entity
{
    public bool IsFavourite { get; set; }
    public DateTime SelectedAt { get; set; } = DateTime.UtcNow;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
}

[Table("customer_loyalties")]
public class CustomerLoyalty : Entity
{
    public int Points { get; set; }
    public LoyaltyType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? RemainingPurchasesExpiration { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
}

[Table("favourites")]
[Index(nameof(CustomerId), nameof(ArticleId), IsUnique = true)]
public class Favourite : Entity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
}

[Table("notifications")]
public class Notification : Entity
{
    public NotificationType Type { get; set; }
    [MaxLength(200)] public required string Title { get; set; }
    [MaxLength(1000)] public required string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
}
