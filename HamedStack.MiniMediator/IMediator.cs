namespace HamedStack.MiniMediator;

/// <summary>
/// Mediator interface for sending requests and publishing notifications.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request to a single handler and returns a response of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected from the request handler.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the handler's response.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to a single handler that does not return a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of request to send.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

    /// <summary>
    /// Sends a request object to a single handler and returns a response object.
    /// This method uses dynamic dispatch and is useful when the request type is not known at compile time.
    /// </summary>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the handler's response, or null if the request does not return a response.</returns>
    Task<object?> Send(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification object to multiple handlers.
    /// This method uses dynamic dispatch and is useful when the notification type is not known at compile time.
    /// </summary>
    /// <param name="notification">The notification object to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Publish(object notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to multiple handlers.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification to publish.</typeparam>
    /// <param name="notification">The notification object to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}