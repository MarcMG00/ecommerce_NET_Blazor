namespace Ecommerce.Api.Infrastructure;

/// <summary>Se traduce a 404.</summary>
public sealed class NotFoundException(string resource, object key)
    : Exception($"{resource} '{key}' was not found.");

/// <summary>Regla de negocio incumplida; se traduce a 400.</summary>
public sealed class RuleViolationException(string message) : Exception(message);
