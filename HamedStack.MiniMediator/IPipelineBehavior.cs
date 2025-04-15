namespace HamedStack.MiniMediator;

/// <summary>
/// Defines a behavior to be executed in the request pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response from the request.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Handles the request by executing pipeline behavior and calling the next delegate.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The delegate that represents the next behavior or handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation with the handler's response.</returns>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}