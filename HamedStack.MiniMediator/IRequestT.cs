

namespace HamedStack.MiniMediator;

/// <summary>
/// Marker interface for requests that return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the request handler.</typeparam>
public interface IRequest<out TResponse> : IBaseRequest;