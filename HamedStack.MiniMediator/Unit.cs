namespace HamedStack.MiniMediator;

/// <summary>
/// Represents a void type, or a type with a single value.
/// </summary>
public struct Unit
{
    /// <summary>
    /// Gets the single value of the Unit type.
    /// </summary>
    /// <returns>The single value of the Unit type.</returns>
    public static Unit Value => new();
}