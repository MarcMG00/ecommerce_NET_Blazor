namespace Ecommerce.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleImage> ArticleImages => Set<ArticleImage>();
    public DbSet<ArticlePromotion> ArticlePromotions => Set<ArticlePromotion>();
    public DbSet<ArticleStore> ArticleStores => Set<ArticleStore>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerLoyalty> CustomerLoyalties => Set<CustomerLoyalty>();
    public DbSet<CustomerStore> CustomerStores => Set<CustomerStore>();
    public DbSet<Favourite> Favourites => Set<Favourite>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<OrderPreparer> OrderPreparers => Set<OrderPreparer>();
    public DbSet<PickupReservation> PickupReservations => Set<PickupReservation>();
    public DbSet<PickupSchedule> PickupSchedules => Set<PickupSchedule>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();
    public DbSet<PurchaseList> PurchaseLists => Set<PurchaseList>();
    public DbSet<PurchaseListElement> PurchaseListElements => Set<PurchaseListElement>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<ShoppingCartLine> ShoppingCartLines => Set<ShoppingCartLine>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Vendor> Vendors => Set<Vendor>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        // Importes: numeric(10,2), igual que precision/scale del modelo Java.
        builder.Properties<decimal>().HavePrecision(10, 2);

        // Enums guardados como texto (equivale a @Enumerated(EnumType.STRING)).
        builder.Properties<Role>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<PromotionType>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<LoyaltyType>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<NotificationType>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<OrderState>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<PurchaseState>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<ReservationState>().HaveConversion<string>().HaveMaxLength(30);
        builder.Properties<PurchaseListElementType>().HaveConversion<string>().HaveMaxLength(20);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // El Id lo asigna siempre el código (Guid v7). Con ValueGeneratedNever, EF trata cualquier
        // entidad nueva descubierta por navegación como "Added" y no como "ya existente".
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(Entity).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType).Property(nameof(Entity.Id)).ValueGeneratedNever();
        }

        // Por defecto nada se borra en cascada (como en JPA)...
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // ...salvo las líneas/hijos que no tienen sentido sin su padre.
        modelBuilder.Entity<Article>().HasMany(a => a.Images).WithOne(i => i.Article).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ShoppingCart>().HasMany(c => c.Lines).WithOne(l => l.ShoppingCart).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Purchase>().HasMany(p => p.Lines).WithOne(l => l.Purchase).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Order>().HasMany(o => o.Lines).WithOne(l => l.Order).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PurchaseList>().HasMany(l => l.Elements).WithOne(e => e.PurchaseList).OnDelete(DeleteBehavior.Cascade);
    }
}
