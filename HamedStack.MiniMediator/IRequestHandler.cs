namespace HamedStack.MiniMediator;

/// <summary>
/// Handles a request and returns a response.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response.</typeparam>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The response.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}