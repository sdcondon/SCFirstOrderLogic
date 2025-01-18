// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic;

/// <summary>
/// Singleton representation of the identifier of the equality predicate. In typical FOL syntax, this is written as:
/// <code>{term} = {term}</code>
/// </summary>
public sealed class EqualityIdentifier
{
    private EqualityIdentifier() {}

    /// <summary>
    /// Gets the singleton instance of <see cref="EqualityIdentifier"/>.
    /// </summary>
    public static EqualityIdentifier Instance { get; } = new EqualityIdentifier();

    /// <inheritdoc />
    public override string ToString() => "Equals";
}
