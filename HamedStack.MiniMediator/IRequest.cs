namespace HamedStack.MiniMediator;

/// <summary>
/// Represents a request that returns a response.
/// </summary>
/// <typeparam name="TResponse">The type of response.</typeparam>
public interface IRequest<out TResponse> { }