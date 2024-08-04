// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// A <see cref="IComparer{T}"/> implementation that uses hash code for comparison,
/// but also makes an arbitrary but consistent decision when two distinct objects
/// have colliding hash codes. This results in a comparer that defines a "less than
/// or equal" relation that is antisymmetric - and is thus usable by feature vector indices
/// for feature comparison. It is however important to note that these arbitrary decisions
/// will not be the same across runs (or even across instances). As such, this type
/// should NOT be used when the "same" index is used across runs - that is, when any
/// kind of persistence is involved. And of course it also can't be used if the hash code
/// semantics aren't appropriate - if, for example, hash code is reference based, but we
/// are going to be looking stuff up that and expecting to match based on value.
/// </summary>
public class CollisionResolvingHashCodeComparer<T> : IComparer<T>
    where T : notnull
{
    private static readonly IComparer<int> IntComparer = Comparer<int>.Default;

    private readonly IEqualityComparer<T> equalityComparer;
    private readonly Dictionary<(T, T), int> collisionResolutions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionResolvingHashCodeComparer{T}"/> class.
    /// </summary>
    public CollisionResolvingHashCodeComparer()
        : this(EqualityComparer<T>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionResolvingHashCodeComparer{T}"/> class.
    /// </summary>
    /// <param name="equalityComparer">The equality comparer to use to obtain hash codes and check for equality.</param>
    public CollisionResolvingHashCodeComparer(IEqualityComparer<T> equalityComparer)
    {
        this.equalityComparer = equalityComparer;
        CollisionResolutions = new ReadOnlyDictionary<(T, T), int>(collisionResolutions);
    }

    /// <summary>
    /// Gets the resolutions that the comparer has made to hash code collisions.
    /// Encountered distinct object pairs with colliding hash codes are present as keys.
    /// The associated value is the arbitrarily decided upon comparison of the two objects.
    /// </summary>
    // TODO: not sure why ive bothered making this public, given that ive not also allowed
    // it to be set on instantiation (so doesn't help anyone wanting to, say, serialise this
    // alongside a index instance).
    public IReadOnlyDictionary<(T, T), int> CollisionResolutions { get; }

#pragma warning disable CS8767 // This is fine - T has notnull constraint
    /// <inheritdoc/>
    public int Compare(T x, T y)
    {
        var comparison = IntComparer.Compare(equalityComparer.GetHashCode(x), equalityComparer.GetHashCode(y));

        if (comparison == 0 && !equalityComparer.Equals(x, y) && !collisionResolutions.TryGetValue((x, y), out comparison))
        {
            // TODO: no thread-safety. Could use concurrentdict and store one way around only - of
            // course then need two TryGets, with different comparison result depending on which
            // one succeeds.
            comparison = collisionResolutions[(x, y)] = 1;
            collisionResolutions[(y, x)] = -1;
        }

        return comparison;
    }
#pragma warning restore CS8767
}
