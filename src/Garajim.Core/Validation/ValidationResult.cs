namespace Garajim.Core.Validation;

public sealed record ValidationResult(IReadOnlyList<string> Errors)
{
    public static ValidationResult Success { get; } = new(Array.Empty<string>());

    public bool IsValid => Errors.Count == 0;
}
