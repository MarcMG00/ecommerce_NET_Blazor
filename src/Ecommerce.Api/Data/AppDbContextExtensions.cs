namespace Ecommerce.Api.Data;

public static class AppDbContextExtensions
{
    // Extension members de C# 14.
    extension(AppDbContext db)
    {
        /// <summary>Lanza <see cref="NotFoundException"/> (404) si el cliente no existe.</summary>
        public async Task EnsureCustomerExistsAsync(Guid customerId, CancellationToken ct)
        {
            if (!await db.Customers.AnyAsync(c => c.Id == customerId, ct))
            {
                throw new NotFoundException(nameof(Customer), customerId);
            }
        }
    }
}
