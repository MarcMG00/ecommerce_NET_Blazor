namespace Ecommerce.Api.Domain;

/// <summary>Clave Guid v7 generada en cliente (ordenable por tiempo, buena para índices).</summary>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
