using Microsoft.Extensions.DependencyInjection;

namespace HamedStack.MiniMediator
{
    /// <summary>
    /// Default implementation of <see cref="IMediator"/> using dependency injection.
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="provider">The service provider used for resolving handlers.</param>
        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _provider.GetRequiredService(handlerType);

            var behaviors = _provider.GetServices(typeof(IPipelineBehavior<,>)
                    .MakeGenericType(request.GetType(), typeof(TResponse)))
                .Cast<IPipelineBehavior<IRequest<TResponse>, TResponse>>()
                .Reverse()
                .ToList();

            RequestHandlerDelegate<TResponse> handlerDelegate = () => (Task<TResponse>)handlerType
                .GetMethod("Handle")!
                .Invoke(handler, [request, cancellationToken])!;

            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;
                handlerDelegate = () => behavior.Handle((IRequest<TResponse>)request, cancellationToken, next);
            }

            return await handlerDelegate();
        }

        /// <inheritdoc />
        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            var handlers = _provider.GetServices<INotificationHandler<TNotification>>();
            var tasks = handlers.Select(h => h.Handle(notification, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }
}
