# HamedStack.MiniMediator

**HamedStack.MiniMediator** is a lightweight, zero-dependency, performance-friendly mediator library built for .NET. It provides an abstraction for **sending commands/queries** and **publishing events** in a clean, decoupled way — inspired by MediatR but simplified for minimal overhead, making it easy to replace **MediatR** if you're looking for something more lightweight and flexible.

---

## 📦 Features

- ✅ Request/Response (Command-Query) pattern
- ✅ Notification pattern (Pub/Sub to multiple handlers)
- ✅ Pipeline behaviors (Middleware support)
- ✅ DI-ready with `IServiceCollection` extension
- ✅ Runtime dynamic dispatch support
- ✅ No external package dependencies
- ✅ Similar API to **MediatR**, making it easy to replace

---

## 🚀 Quick Start

### 1. Define a Request + Handler

```csharp
public class Ping : IRequest<string>
{
    public string Message { get; set; }
}

public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}
```

### 2. Define a Notification + Handler

```csharp
public class UserCreated : INotification
{
    public string UserName { get; set; }
}

public class SendWelcomeEmail : INotificationHandler<UserCreated>
{
    public Task Handle(UserCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Welcome email sent to {notification.UserName}");
        return Task.CompletedTask;
    }
}
```

---

## 🛠 Registering in Dependency Injection

You can register the **MiniMediator** services into your DI container by calling `AddMiniMediator`.

### Registering with specific assemblies:

```csharp
builder.Services.AddMiniMediator(typeof(Program).Assembly);
```

This will scan the given assembly for any **request handlers** and **notification handlers**.

### Registering with **all assemblies** in the current AppDomain:

If you don’t provide any assemblies, **MiniMediator** will automatically scan all non-system assemblies in the current AppDomain for handler implementations. 

```csharp
builder.Services.AddMiniMediator();
```

This is useful when you want to automatically discover and register all handlers in the app, eliminating the need to manually specify each assembly.

---

## ⚙️ Using IMediator

```csharp
public class SomeService
{
    private readonly IMediator _mediator;

    public SomeService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DoSomething()
    {
        // Sending request and getting response
        var result = await _mediator.Send(new Ping { Message = "Hello" });
        Console.WriteLine(result); // Output: Pong: Hello

        // Publishing a notification
        await _mediator.Publish(new UserCreated { UserName = "Hamed" });
    }
}
```

---

## 🧱 Pipeline Behavior Example

Create a behavior to log request processing times:

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        Console.WriteLine($"[START] Handling {name}");

        var response = await next();

        Console.WriteLine($"[END] Finished {name}");

        return response;
    }
}
```

### 🔧 Registering Pipeline Behaviors

Add them to your DI container:

```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

Pipeline behaviors can be stacked, and they execute in the order they are registered (outermost to innermost).

---

## 💡 Advanced Use: Dynamic Dispatch

You can send dynamically typed requests or notifications:

```csharp
object dynamicRequest = new Ping { Message = "Dynamic Hello" };
var result = await _mediator.Send(dynamicRequest); // returns object

object dynamicNotification = new UserCreated { UserName = "DynamicUser" };
await _mediator.Publish(dynamicNotification);
```

---

## 📚 Interfaces Overview

| Interface | Description |
|----------|-------------|
| `IRequest` | Marker for requests with no return value |
| `IRequest<TResponse>` | Marker for requests with a return value |
| `INotification` | Marker for notifications (pub-sub) |
| `IRequestHandler<TRequest>` | Handler for void requests |
| `IRequestHandler<TRequest, TResponse>` | Handler for response-returning requests |
| `INotificationHandler<TNotification>` | Handler for notifications |
| `IPipelineBehavior<TRequest, TResponse>` | Middleware behavior in the pipeline |
| `IMediator` | Mediator interface to send/publish |
