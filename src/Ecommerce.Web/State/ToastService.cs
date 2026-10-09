namespace Ecommerce.Web.State;

/// <summary>
/// Enum for toast
/// </summary>
public enum ToastKind { Success, Error }

/// <summary>
/// Toast constructor
/// </summary>
/// <param name="Message"></param>
/// <param name="Kind"></param>
public sealed record Toast(string Message, ToastKind Kind)
{
    public Guid Id { get; } = Guid.NewGuid();
}

/// <summary>
/// Events from Toast
/// </summary>
public sealed class ToastService
{
    /// <summary>
    /// Alerts that class has changed => from elements subscribed
    /// </summary>
    public event Action<Toast>? Shown;

    /// <summary>
    /// Display successfull message
    /// </summary>
    /// <param name="message"></param>
    public void Success(string message) => Shown?.Invoke(new Toast(message, ToastKind.Success));

    /// <summary>
    /// Display error message
    /// </summary>
    /// <param name="message"></param>
    public void Error(string message) => Shown?.Invoke(new Toast(message, ToastKind.Error));
}
