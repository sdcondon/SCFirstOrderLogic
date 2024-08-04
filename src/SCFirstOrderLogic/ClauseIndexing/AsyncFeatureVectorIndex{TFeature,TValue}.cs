// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// <para>
/// An implementation of a feature vector index for <see cref="CNFClause"/>s.
/// </para>
/// <para>
/// Feature vector indexing is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public class AsyncFeatureVectorIndex<TFeature, TValue>
    where TFeature : notnull
{
    private static readonly IEnumerable<KeyValuePair<CNFClause, TValue>> EmptyElements = Enumerable.Empty<KeyValuePair<CNFClause, TValue>>();

    private readonly Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector;
    private readonly IAsyncFeatureVectorIndexNode<TFeature, TValue> root;
    private readonly IComparer<TFeature> elementComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector)
        : this(featureVectorSelector, Comparer<TFeature>.Default, new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer)
        : this(featureVectorSelector, featureComparer, new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IAsyncFeatureVectorIndexNode<TFeature, TValue> root)
        : this(featureVectorSelector, Comparer<TFeature>.Default, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some initial content,
    /// that uses the default comparer of the key element type to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="content">The initial content to be added to the tree.</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, Comparer<TFeature>.Default, new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a 
    /// specified root node and no (additional) initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IAsyncFeatureVectorIndexNode<TFeature, TValue> root)
        : this(featureVectorSelector, featureComparer, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="AsyncFeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, featureComparer, new AsyncFeatureVectorIndexDictionaryNode<TFeature, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default comparer
    /// of the key element type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IAsyncFeatureVectorIndexNode<TFeature, TValue> root,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, Comparer<TFeature>.Default, root, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncFeatureVectorIndex{TFeature,TValue}"/> class with a 
    /// specified root node and some (additional) initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public AsyncFeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IAsyncFeatureVectorIndexNode<TFeature, TValue> root,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
    {
        ArgumentNullException.ThrowIfNull(featureVectorSelector);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(featureComparer);
        ArgumentNullException.ThrowIfNull(content);

        this.featureVectorSelector = featureVectorSelector;
        this.root = root;
        this.elementComparer = featureComparer;

        foreach (var kvp in content)
        {
            AddAsync(kvp.Key, kvp.Value).GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Adds a clause and associated value to the index.
    /// </summary>
    /// <param name="key">The clause to add.</param>
    /// <param name="value">The value to associate with the clause.</param>
    public async Task AddAsync(CNFClause key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = featureVectorSelector(key);
        // TODO: sort keyelements by key then by value

        var currentNode = root;
        foreach (var keyElement in keyElements)
        {
            currentNode = await currentNode.GetOrAddChildAsync(keyElement);
        }

        await currentNode.AddValueAsync(value);
    }

    /// <summary>
    /// Removes a clause from the index.
    /// </summary>
    /// <param name="key">The clause to remove.</param>
    /// <returns>A value indicating whether the clause was present prior to this operation.</returns>
    public async Task<bool> RemoveAsync(CNFClause key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = featureVectorSelector(key);
        // TODO: sort keyelements by key then by value

        return await ExpandNodeAsync(root, 0);

        async ValueTask<bool> ExpandNodeAsync(IAsyncFeatureVectorIndexNode<TFeature, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                var keyElement = keyElements[keyElementIndex];
                var childNode = await node.TryGetChildAsync(keyElement);

                if (childNode == null || !await ExpandNodeAsync(childNode, keyElementIndex + 1))
                {
                    return false;
                }

                if (!await childNode.GetChildren().GetAsyncEnumerator().MoveNextAsync() && !childNode.HasValue)
                {
                    await node.DeleteChildAsync(keyElement);
                }

                return true;
            }
            else
            {
                if (!node.HasValue)
                {
                    return false;
                }

                await node.RemoveValueAsync();
                return true;
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a clause.
    /// </summary>
    /// <param name="key">The clause to retrieve the associated value of.</param>
    /// <returns>A task that returns a value indicating whether it was successful, and if so what the retrieved value is.</returns>
    public async Task<(bool isSucceeded, TValue? value)> TryGetAsync(CNFClause key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = featureVectorSelector(key);
        // TODO: sort keyelements by key then by value

        var currentNode = root;
        foreach (var keyElement in keyElements)
        {
            var childNode = await currentNode.TryGetChildAsync(keyElement);
            if (childNode != null)
            {
                currentNode = childNode;
            }
            else
            {
                return (false, default);
            }
        }

        if (currentNode.HasValue)
        {
            return (true, currentNode.Value);
        }

        return (false, default);
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored subset of a given set.
    /// </summary>
    /// <param name="key">The values associated with the stored subsets of this set will be retrieved.</param>
    /// <returns>An async enumerable of the values associated with each stored subset of the given set.</returns>
    public IAsyncEnumerable<TValue> GetSubsets(CNFClause key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = featureVectorSelector(key);
        // TODO: sort keyelements by key then by value

        return ExpandNode(root, 0);

        async IAsyncEnumerable<TValue> ExpandNode(IAsyncFeatureVectorIndexNode<TFeature, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                var childNode = await node.TryGetChildAsync(keyElements[keyElementIndex]);

                if (childNode != null)
                {
                    await foreach (var value in ExpandNode(childNode, keyElementIndex + 1))
                    {
                        yield return value;
                    }
                }

                await foreach (var value in ExpandNode(node, keyElementIndex + 1))
                {
                    yield return value;
                }
            }
            else
            {
                if (node.HasValue)
                {
                    yield return node.Value;
                }
            }
        }
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored superset a given set.
    /// </summary>
    /// <param name="key">The values associated with the stored supersets of this set will be retrieved.</param>
    /// <returns>An async enumerable of the values associated with each stored superset a given set.</returns>
    public IAsyncEnumerable<TValue> GetSupersets(CNFClause key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var keyElements = featureVectorSelector(key);
        // TODO: sort keyelements by key then by value

        return ExpandNode(root, 0);

        async IAsyncEnumerable<TValue> ExpandNode(IAsyncFeatureVectorIndexNode<TFeature, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < keyElements.Length)
            {
                await foreach (var (childKeyElement, childNode) in node.GetChildren())
                {
                    if (keyElementIndex == 0 || elementComparer.Compare(childKeyElement, keyElements[keyElementIndex - 1]) > 0)
                    {
                        var childComparedToCurrent = elementComparer.Compare(childKeyElement, keyElements[keyElementIndex]);

                        if (childComparedToCurrent <= 0)
                        {
                            var keyElementIndexOffset = childComparedToCurrent == 0 ? 1 : 0;

                            await foreach (var value in ExpandNode(childNode, keyElementIndex + keyElementIndexOffset))
                            {
                                yield return value;
                            }
                        }
                    }
                }
            }
            else
            {
                await foreach (var value in GetAllDescendentValues(node))
                {
                    yield return value;
                }
            }
        }

        async IAsyncEnumerable<TValue> GetAllDescendentValues(IAsyncFeatureVectorIndexNode<TFeature, TValue> node)
        {
            if (node.HasValue)
            {
                yield return node.Value;
            }

            await foreach (var (_, childNode) in node.GetChildren())
            {
                await foreach (var value in GetAllDescendentValues(childNode))
                {
                    yield return value;
                }
            }
        }
    }
}
