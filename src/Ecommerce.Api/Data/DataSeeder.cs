namespace Ecommerce.Api.Data;

/// <summary>
/// Rellena la BD con los datos de scripts/insert_queries_start_project.sql (mismos Guids)
/// más un cliente de demo con un favorito y un carrito. Es idempotente: si ya hay tiendas, no hace nada.
/// </summary>
public sealed class DataSeeder(AppDbContext db, ILogger<DataSeeder> logger)
{
    // 1xxxxxxx = tiendas, 2 = categorías, 3 = artículos, 4 = articles_store, 5 = promociones, 6 = article_promotions
    private static Guid Id(int kind, int number) => new($"{kind}0000000-0000-0000-0000-{number:D12}");

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Stores.AnyAsync(ct))
        {
            logger.LogInformation("La BD ya contiene datos; se omite el seeding.");
            return;
        }

        var now = DateTime.UtcNow;

        Store[] stores =
        [
            new() { Id = Id(1, 1), Name = "Supermercado Central", Address = "Calle Mayor 10", City = "Madrid", ZipCode = "28001", Phone = "910000001" },
            new() { Id = Id(1, 2), Name = "Supermercado Norte", Address = "Avenida Norte 25", City = "Madrid", ZipCode = "28015", Phone = "910000002" },
            new() { Id = Id(1, 3), Name = "Supermercado Sur", Address = "Calle Sur 8", City = "Madrid", ZipCode = "28020", Phone = "910000003" },
        ];

        Category[] categories =
        [
            new() { Id = Id(2, 1), Name = "Lácteos", Description = "Leche, yogures, quesos y otros productos lácteos." },
            new() { Id = Id(2, 2), Name = "Bebidas", Description = "Agua, refrescos, zumos y otras bebidas." },
            new() { Id = Id(2, 3), Name = "Desayuno", Description = "Cereales, galletas y productos para el desayuno." },
            new() { Id = Id(2, 4), Name = "Frutas y verduras", Description = "Frutas y verduras frescas." },
            new() { Id = Id(2, 5), Name = "Snacks", Description = "Patatas, frutos secos y otros aperitivos." },
        ];

        Article[] articles =
        [
            new() { Id = Id(3, 1), Name = "Leche entera", Description = "Leche entera de vaca, envase de 1 litro.", Brand = "Lácteos Madrid", BarCode = "841000000001", CategoryId = Id(2, 1) },
            new() { Id = Id(3, 2), Name = "Yogur natural", Description = "Pack de 4 yogures naturales.", Brand = "La Granja", BarCode = "841000000002", CategoryId = Id(2, 1) },
            new() { Id = Id(3, 3), Name = "Cereales de chocolate", Description = "Cereales de desayuno con sabor a chocolate.", Brand = "ChocoMax", BarCode = "841000000003", CategoryId = Id(2, 3) },
            new() { Id = Id(3, 4), Name = "Agua mineral", Description = "Botella de agua mineral de 1,5 litros.", Brand = "Fuente Clara", BarCode = "841000000004", CategoryId = Id(2, 2) },
            new() { Id = Id(3, 5), Name = "Zumo de naranja", Description = "Zumo de naranja 100% exprimido.", Brand = "Naranjalia", BarCode = "841000000005", CategoryId = Id(2, 2) },
            new() { Id = Id(3, 6), Name = "Manzanas Golden", Description = "Manzanas Golden seleccionadas.", Brand = "Frutas del Campo", BarCode = "841000000006", CategoryId = Id(2, 4) },
            new() { Id = Id(3, 7), Name = "Patatas fritas", Description = "Patatas fritas clásicas, bolsa de 150 gramos.", Brand = "Croc", BarCode = "841000000007", CategoryId = Id(2, 5) },
            new() { Id = Id(3, 8), Name = "Galletas de chocolate", Description = "Galletas rellenas de chocolate.", Brand = "DulceCasa", BarCode = "841000000008", CategoryId = Id(2, 3) },
        ];

        // (artículo, tienda, precio, stock) -> ids 4...0001 a 4...0019, en el mismo orden que el script SQL.
        (int Article, int Store, decimal Price, int Stock)[] offers =
        [
            (1, 1, 1.49m, 15), (1, 2, 1.55m, 0), (1, 3, 1.45m, 8),
            (2, 1, 2.10m, 10), (2, 2, 1.95m, 5),
            (3, 1, 2.79m, 8), (3, 2, 2.69m, 12), (3, 3, 2.89m, 0),
            (4, 1, 0.65m, 30), (4, 2, 0.60m, 20), (4, 3, 0.70m, 0),
            (5, 1, 2.49m, 6), (5, 3, 2.39m, 4),
            (6, 1, 2.20m, 15), (6, 2, 2.10m, 0),
            (7, 1, 1.80m, 20), (7, 2, 1.75m, 10),
            (8, 1, 1.99m, 0), (8, 2, 1.89m, 7),
        ];

        var articleStores = offers
            .Select((o, i) => new ArticleStore
            {
                Id = Id(4, i + 1),
                ArticleId = Id(3, o.Article),
                StoreId = Id(1, o.Store),
                Price = o.Price,
                Stock = o.Stock,
                UpdatedAt = now,
            })
            .ToList();

        Promotion[] promotions =
        [
            new() { Id = Id(5, 1), Name = "Descuento leche", Description = "10% de descuento en leche entera.", StartDate = now, EndDate = now.AddDays(30), Type = PromotionType.PercentageDiscount, Value = 10.00m },
            new() { Id = Id(5, 2), Name = "Oferta cereales", Description = "50 céntimos de descuento.", StartDate = now, EndDate = now.AddDays(15), Type = PromotionType.AmountDiscount, Value = 0.50m },
        ];

        ArticlePromotion[] articlePromotions =
        [
            new() { Id = Id(6, 1), ArticleStoreId = Id(4, 1), PromotionId = Id(5, 1) },
            new() { Id = Id(6, 2), ArticleStoreId = Id(4, 6), PromotionId = Id(5, 2) },
        ];

        // Franjas de recogida de los próximos 5 días (2 por tienda y día).
        var today = DateOnly.FromDateTime(now);
        TimeOnly[] slotStarts = [new(10, 0), new(16, 0)];
        List<PickupSchedule> schedules = [];

        foreach (var store in stores)
        {
            for (var day = 1; day <= 5; day++)
            {
                foreach (var start in slotStarts)
                {
                    schedules.Add(new PickupSchedule
                    {
                        StoreId = store.Id,
                        Date = today.AddDays(day),
                        StartTime = start,
                        EndTime = start.AddHours(2),
                        Places = 10,
                        RemainingPlaces = 10,
                    });
                }
            }
        }

        // Cliente de demo: el mismo Guid que usaba el frontend Angular.
        var user = new User
        {
            Id = Id(7, 1),
            Email = "demo.customer@ecommerce.local",
            PasswordHash = PasswordHasher.Hash("Demo1234!"),
            Role = Role.Customer,
        };

        var customer = new Customer
        {
            Id = DemoData.CustomerId,
            UserId = user.Id,
            FirstName = "Demo",
            LastName = "Customer",
            Phone = "600000000",
            StoreReferenceId = stores[0].Id,
            CreatedAt = now,
        };

        var favourite = new Favourite { CustomerId = customer.Id, ArticleId = Id(3, 1), CreatedAt = now };

        var cart = new ShoppingCart { CustomerId = customer.Id, CreatedAt = now, UpdatedAt = now };
        var cartLine = new ShoppingCartLine
        {
            ShoppingCartId = cart.Id,
            ArticleStoreId = Id(4, 4), // yogur natural en Supermercado Central
            Quantity = 2,
            UnitPrice = 2.10m,
        };

        db.Stores.AddRange(stores);
        db.Categories.AddRange(categories);
        db.Articles.AddRange(articles);
        db.ArticleStores.AddRange(articleStores);
        db.Promotions.AddRange(promotions);
        db.ArticlePromotions.AddRange(articlePromotions);
        db.PickupSchedules.AddRange(schedules);
        db.Users.Add(user);
        db.Customers.Add(customer);
        db.Favourites.Add(favourite);
        db.ShoppingCarts.Add(cart);
        db.ShoppingCartLines.Add(cartLine);

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "Seeding completado: {Stores} tiendas, {Articles} artículos, {Offers} precios por tienda.",
            stores.Length, articles.Length, articleStores.Count);
    }
}
