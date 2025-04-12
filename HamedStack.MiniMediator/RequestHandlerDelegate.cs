namespace HamedStack.MiniMediator;

/// <summary>
/// Delegate representing the next step in a request pipeline.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <returns>The response task.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();