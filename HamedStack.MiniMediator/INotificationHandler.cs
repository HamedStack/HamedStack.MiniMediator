namespace HamedStack.MiniMediator;

/// <summary>
/// Handles a notification.
/// </summary>
/// <typeparam name="TNotification">The type of notification.</typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    /// <summary>
    /// Handles the notification.
    /// </summary>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}