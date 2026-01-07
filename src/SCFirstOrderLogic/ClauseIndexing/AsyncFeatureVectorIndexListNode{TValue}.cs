// Copyright (c) 2023-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaManipulation.Substitution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

#pragma warning disable CS1998 // async lacks await. See 'NB' in class summary.
/// <summary>
/// <para>
/// An implementation of <see cref="IAsyncFeatureVectorIndexNode{TValue}"/> that just stores its content in memory.
/// Uses a <see cref="SortedList{TKey, TValue}"/> for child nodes, and a <see cref="Dictionary{TKey, TValue}"/> for leaf values.
/// </para>
/// <para>
/// NB: If you are using this type, you should consider using <see cref="FeatureVectorIndex{TValue}"/> instead, to avoid the overhead of asynchronicity.
/// <see cref="AsyncFeatureVectorIndex{TValue}"/> is intended for use with node implementations that interact with external (i.e. I/O-requiring) storage.
/// This type is intended only as an example implementation to base real implementations on.
/// </para>
/// </summary>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class AsyncFeatureVectorIndexListNode<TValue> : IAsyncFeatureVectorIndexNode<TValue>
{
    private readonly SortedList<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>> childrenByVectorComponent;
    private readonly Dictionary<CNFClause, TValue> valuesByKey = new(new VariableIdAgnosticEqualityComparer());

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexListNode{TValue}"/> class that
    /// uses the default comparer of the feature type to determine the ordering of nodes. Note that this comparer will
    /// throw if the runtime type of a feature object does not implement <see cref="IComparable{T}"/>.
    /// </summary>
    public AsyncFeatureVectorIndexListNode()
        : this(Comparer.Default)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AsyncFeatureVectorIndexListNode{TValue}"/> class.
    /// </summary>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of nodes. NB: For correct behaviour, the index must be able to
    /// unambiguously order the components of a feature vector. As such, this comparer must only return zero for equal 
    /// features (and of course duplicates shouldn't occur in any given vector).
    /// </param>
    public AsyncFeatureVectorIndexListNode(IComparer featureComparer)
    {
        FeatureComparer = featureComparer;
        childrenByVectorComponent = new(new FeatureVectorComponentComparer(featureComparer));
    }

    private AsyncFeatureVectorIndexListNode(IComparer featureComparer, IComparer<FeatureVectorComponent> vectorComponentComparer)
    {
        FeatureComparer = featureComparer;
        childrenByVectorComponent = new(vectorComponentComparer);
    }

    /// <inheritdoc/>
    public IComparer FeatureComparer { get; }

    /// <inheritdoc/>
    public IAsyncEnumerable<KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>> ChildrenAscending
    {
        get
        {
            async IAsyncEnumerable<KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>> GetReturnValue()
            {
                foreach (var kvp in childrenByVectorComponent)
                {
                    yield return kvp;
                }
            }

            return GetReturnValue();
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>> ChildrenDescending
    {
        get
        {
            async IAsyncEnumerable<KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>> GetReturnValue()
            {
                for (int i = childrenByVectorComponent.Count - 1; i >= 0; i--)
                {
                    yield return new KeyValuePair<FeatureVectorComponent, IAsyncFeatureVectorIndexNode<TValue>>(childrenByVectorComponent.Keys[i], childrenByVectorComponent.Values[i]);
                }
            }

            return GetReturnValue();
        }
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> KeyValuePairs
    {
        get
        {
            async IAsyncEnumerable<KeyValuePair<CNFClause, TValue>> GetReturnValue()
            {
                foreach (var kvp in valuesByKey)
                {
                    yield return kvp;
                }
            }

            return GetReturnValue();
        }
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TValue>?> TryGetChildAsync(FeatureVectorComponent vectorComponent, CancellationToken cancellationToken = default)
    {
        childrenByVectorComponent.TryGetValue(vectorComponent, out var child);
        return ValueTask.FromResult(child);
    }

    /// <inheritdoc/>
    public ValueTask<IAsyncFeatureVectorIndexNode<TValue>> GetOrAddChildAsync(FeatureVectorComponent vectorComponent, CancellationToken cancellationToken = default)
    {
        IAsyncFeatureVectorIndexNode<TValue> node = new AsyncFeatureVectorIndexListNode<TValue>(FeatureComparer, childrenByVectorComponent.Comparer);
        if (!childrenByVectorComponent.TryAdd(vectorComponent, node))
        {
            node = childrenByVectorComponent[vectorComponent];
        }

        return ValueTask.FromResult(node);
    }

    /// <inheritdoc/>
    public ValueTask DeleteChildAsync(FeatureVectorComponent vectorComponent, CancellationToken cancellationToken = default)
    {
        childrenByVectorComponent.Remove(vectorComponent, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask AddValueAsync(CNFClause clause, TValue value, CancellationToken cancellationToken = default)
    {
        if (!valuesByKey.TryAdd(clause, value))
        {
            throw new ArgumentException("Key already present", nameof(clause));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask<bool> RemoveValueAsync(CNFClause clause, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(valuesByKey.Remove(clause));
    }

    /// <inheritdoc/>
    public ValueTask<(bool isSucceeded, TValue? value)> TryGetValueAsync(CNFClause clause, CancellationToken cancellationToken = default)
    {
        var isSucceeded = valuesByKey.TryGetValue(clause, out var value);
        return ValueTask.FromResult((isSucceeded, value));
    }
}
#pragma warning restore CS1998 // async lacks await. See 'NB' in class summary.
