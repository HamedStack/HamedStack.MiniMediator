namespace HamedStack.MiniMediator;

/// <summary>
/// Mediator interface for sending requests and publishing notifications.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The response from the appropriate handler.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}