// ReSharper disable UnusedMember.Global
// ReSharper disable UseCollectionExpression
// ReSharper disable UnusedMemberInSuper.Global

using Microsoft.Extensions.DependencyInjection;

namespace HamedStack.MiniMediator;

/// <summary>
/// Default implementation of the <see cref="IMediator"/> interface.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve handlers and behaviors.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Sends a request to a single handler and returns a response of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of response expected from the request handler.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the handler's response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the request type.</exception>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType)
                      ?? throw new InvalidOperationException($"Handler for {requestType.Name} not registered");
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<IRequest<TResponse>, TResponse>>();
        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var handleMethod = handlerType.GetMethod("Handle");
            return (Task<TResponse>)handleMethod?.Invoke(handler, new object[] { request, cancellationToken })!;
        };
        foreach (var behavior in behaviors.Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }
        return await pipeline();
    }

    /// <summary>
    /// Sends a request to a single handler that does not return a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of request to send.</typeparam>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the request type.</exception>
    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = _serviceProvider.GetService(handlerType)
                      ?? throw new InvalidOperationException($"Handler for {requestType.Name} not registered");
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TRequest, Unit>>();
        RequestHandlerDelegate<Unit> pipeline = async () =>
        {
            var handleMethod = handlerType.GetMethod("Handle");
            await (Task)handleMethod?.Invoke(handler, new object[] { request, cancellationToken })!;
            return Unit.Value;
        };
        foreach (var behavior in behaviors.Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }
        await pipeline();
    }

    /// <summary>
    /// Sends a request object to a single handler and returns a response object.
    /// This method uses dynamic dispatch and is useful when the request type is not known at compile time.
    /// </summary>
    /// <param name="request">The request object to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the handler's response, or null if the request does not return a response.</returns>
    public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var requestInterfaceType = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
        if (requestInterfaceType != null)
        {
            var responseType = requestInterfaceType.GetGenericArguments()[0];
            var method = typeof(Mediator).GetMethod(nameof(Send),
                new[] { typeof(IRequest<>).MakeGenericType(responseType), typeof(CancellationToken) });
            var genericMethod = method?.MakeGenericMethod(responseType);
            return await (Task<object>)genericMethod?.Invoke(this, new[] { request, cancellationToken })!;
        }
        else
        {
            var method =
                typeof(Mediator).GetMethod(nameof(Send), new[] { typeof(IRequest), typeof(CancellationToken) });
            var genericMethod = method?.MakeGenericMethod(requestType);
            await (Task)genericMethod?.Invoke(this, new[] { request, cancellationToken })!;
            return null;
        }
    }

    /// <summary>
    /// Publishes a notification to multiple handlers.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification to publish.</typeparam>
    /// <param name="notification">The notification object to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish<TNotification>(TNotification notification,
        CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        var handlers = _serviceProvider.GetServices(handlerType);
        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("Handle");
            tasks.Add((Task)handleMethod?.Invoke(handler, new object[] { notification, cancellationToken })!);
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Publishes a notification object to multiple handlers.
    /// This method uses dynamic dispatch and is useful when the notification type is not known at compile time.
    /// </summary>
    /// <param name="notification">The notification object to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        var notificationType = notification.GetType();
        var method = typeof(Mediator).GetMethod(nameof(Publish),
            new[] { typeof(INotification), typeof(CancellationToken) });
        var genericMethod = method?.MakeGenericMethod(notificationType);
        await (Task)genericMethod?.Invoke(this, new[] { notification, cancellationToken })!;
    }
}