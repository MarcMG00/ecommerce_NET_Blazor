namespace Ecommerce.Api.Domain;

[Table("purchases")]
[Index(nameof(Number), IsUnique = true)]
public class Purchase : Entity
{
    [MaxLength(30)] public required string Number { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public PurchaseState State { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<PurchaseLine> Lines { get; set; } = [];
}

[Table("purchase_lines")]
public class PurchaseLine : Entity
{
    /// <summary>Copia del nombre en el momento de la compra (el artículo puede cambiar después).</summary>
    [MaxLength(200)] public required string ArticleName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }

    public Guid PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    public Guid ArticleStoreId { get; set; }
    public ArticleStore ArticleStore { get; set; } = null!;
}

[Table("pickup_schedules")]
public class PickupSchedule : Entity
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Places { get; set; }
    public int RemainingPlaces { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;
}

[Table("orders")]
[Index(nameof(Number), IsUnique = true)]
public class Order : Entity
{
    [MaxLength(40)] public required string Number { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PreparationStartedAt { get; set; }
    public DateTime? PreparationFinishedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public OrderState State { get; set; }

    public Guid PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public Guid? OrderPreparerId { get; set; }
    public OrderPreparer? OrderPreparer { get; set; }

    public Guid? PickupScheduleId { get; set; }
    public PickupSchedule? PickupSchedule { get; set; }

    public ICollection<OrderLine> Lines { get; set; } = [];
}

[Table("order_lines")]
public class OrderLine : Entity
{
    public int QuantityRequested { get; set; }
    public int QuantityPrepared { get; set; }
    public bool IsPrepared { get; set; }
    public DateTime? PreparedAt { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid PurchaseLineId { get; set; }
    public PurchaseLine PurchaseLine { get; set; } = null!;
}

[Table("pickup_reservations")]
[Index(nameof(OrderId), IsUnique = true)]
public class PickupReservation : Entity
{
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    public ReservationState State { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid PickupScheduleId { get; set; }
    public PickupSchedule PickupSchedule { get; set; } = null!;
}
