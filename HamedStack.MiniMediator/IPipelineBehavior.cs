namespace HamedStack.MiniMediator;

/// <summary>
/// Represents a pipeline behavior that wraps request handling.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request and invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>The response.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
}