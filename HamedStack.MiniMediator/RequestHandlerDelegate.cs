namespace HamedStack.MiniMediator;

/// <summary>
/// Represents a delegate that returns a response from a request handler.
/// </summary>
/// <typeparam name="TResponse">The type of response from the request handler.</typeparam>
/// <returns>A task representing the asynchronous operation with the handler's response.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();