using System.Text.Json.Serialization;

namespace Ecommerce.Shared.Enums;

// All enums are serialized as JSON text and saved as string in DB

/// <summary>
/// User Roles
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Role>))]
public enum Role { Customer, Vendor, OrderPreparer }

/// <summary>
/// Promotion types
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PromotionType>))]
public enum PromotionType { PercentageDiscount, AmountDiscount, SpecialPrice }

/// <summary>
/// Loyalty types
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<LoyaltyType>))]
public enum LoyaltyType { Revenue, Use, Expiration }

/// <summary>
/// Notification types
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<NotificationType>))]
public enum NotificationType { OrderPreparation, OrderReady, ScheduleModification, StoreModification, PaymentMade }

/// <summary>
///  Order states
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<OrderState>))]
public enum OrderState { Pending, InPreparation, Ready, SentToStore, ReadyToPickup, PickedUp, Cancelled }

/// <summary>
/// Purchase states
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PurchaseState>))]
public enum PurchaseState { Confirmed, Paid, InPreparation, Prepared, PickedUp, Cancelled }

/// <summary>
/// Reservation states
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ReservationState>))]
public enum ReservationState { Reserved, Cancelled, Completed }

/// <summary>
/// Purchase list element types (select a category that can be changed to a specific article)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PurchaseListElementType>))]
public enum PurchaseListElementType { Category, Article }
