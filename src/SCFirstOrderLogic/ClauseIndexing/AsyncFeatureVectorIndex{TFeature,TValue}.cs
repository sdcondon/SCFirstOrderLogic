// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.InternalUtilities;
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
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
/// Feature vector indexing (in this context, at least) is an indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class AsyncFeatureVectorIndex<TFeature, TValue>
    where TFeature : notnull
{
    private static readonly IEnumerable<KeyValuePair<CNFClause, TValue>> EmptyElements = Enumerable.Empty<KeyValuePair<CNFClause, TValue>>();

    private readonly Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector;
    private readonly IAsyncFeatureVectorIndexNode<TFeature, TValue> root;
    private readonly IComparer<TFeature> featureComparer;

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
        this.featureComparer = featureComparer;

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

        if (key == CNFClause.Empty)
        {
            throw new ArgumentException("The empty clause is not a valid key", nameof(key));
        }

        var currentNode = root;
        foreach (var element in MakeOrderedFeatureVector(key))
        {
            currentNode = await currentNode.GetOrAddChildAsync(element);
        }

        await currentNode.AddValueAsync(key, value);
    }

    /// <summary>
    /// Removes a clause from the index.
    /// </summary>
    /// <param name="key">The clause to remove.</param>
    /// <returns>A value indicating whether the clause was present prior to this operation.</returns>
    public async Task<bool> RemoveAsync(CNFClause key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var featureVector = MakeOrderedFeatureVector(key);

        return await ExpandNodeAsync(root, 0);

        async ValueTask<bool> ExpandNodeAsync(IAsyncFeatureVectorIndexNode<TFeature, TValue> node, int elementIndex)
        {
            if (elementIndex < featureVector.Count)
            {
                var element = featureVector[elementIndex];
                var childNode = await node.TryGetChildAsync(element);

                if (childNode == null || !await ExpandNodeAsync(childNode, elementIndex + 1))
                {
                    return false;
                }

                if (!await childNode.GetChildren().GetAsyncEnumerator().MoveNextAsync() && !await childNode.GetKeyValuePairs().AnyAsync())
                {
                    await node.DeleteChildAsync(element);
                }

                return true;
            }
            else
            {
                return await node.RemoveValueAsync(key);
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

        var currentNode = root;
        foreach (var element in MakeOrderedFeatureVector(key))
        {
            var childNode = await currentNode.TryGetChildAsync(element);
            if (childNode != null)
            {
                currentNode = childNode;
            }
            else
            {
                return (false, default);
            }
        }

        return await currentNode.TryGetValueAsync(key);
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored clause that subsumes a given clause.
    /// </summary>
    /// <param name="clause">The values associated with the stored clauses that subsume this clause will be retrieved.</param>
    /// <returns>An async enumerable of the values associated with each stored clause that subsumes the given clause.</returns>
    public IAsyncEnumerable<TValue> GetSubsuming(CNFClause clause)
    {
        ArgumentNullException.ThrowIfNull(clause);

        var featureVector = MakeOrderedFeatureVector(clause);

        return ExpandNode(root, 0);

        async IAsyncEnumerable<TValue> ExpandNode(IAsyncFeatureVectorIndexNode<TFeature, TValue> node, int elementIndex)
        {
            if (elementIndex < featureVector.Count)
            {
                // If matching feature with lower value, then recurse
                // todo: can be made more efficient once node children are ordered
                await foreach (var ((childFeature, childMagnitude), childNode) in node.GetChildren())
                {
                    if (childFeature.Equals(featureVector[elementIndex].Key) && childMagnitude <= featureVector[elementIndex].Value)
                    {
                        await foreach (var value in ExpandNode(childNode, elementIndex + 1))
                        {
                            yield return value;
                        }
                    }
                }

                // NB: Matching feature might not be there at all in stored clause (which we interpret
                // as it having value zero) so also just skip the current key element.
                await foreach (var value in ExpandNode(node, elementIndex + 1))
                {
                    yield return value;
                }
            }
            else
            {
                // NB: note that we need to filter the values to those keyed by clauses that actually
                // subsume the query clause. The node values are the candidate set.
                await foreach (var value in node.GetKeyValuePairs().Where(kvp => kvp.Key.Subsumes(clause)).Select(kvp => kvp.Value))
                {
                    yield return value;
                }
            }
        }
    }

    /// <summary>
    /// Returns an enumerable of the values associated with each stored clause that is subsumed by a given clause.
    /// </summary>
    /// <param name="clause">The values associated with the stored clauses that are subsumed by this clause will be retrieved.</param>
    /// <returns>An async enumerable of the values associated with each stored clause that is subsumed by the given clause.</returns>
    public IAsyncEnumerable<TValue> GetSubsumed(CNFClause clause)
    {
        ArgumentNullException.ThrowIfNull(clause);

        var featureVector = MakeOrderedFeatureVector(clause);

        return ExpandNode(root, 0);

        async IAsyncEnumerable<TValue> ExpandNode(IAsyncFeatureVectorIndexNode<TFeature, TValue> node, int elementIndex)
        {
            if (elementIndex < featureVector.Count)
            {
                await foreach (var ((childFeature, childMagnitude), childNode) in node.GetChildren())
                {
                    // todo: is this right? or do we need by feature AND magnitude here?
                    if (elementIndex == 0 || featureComparer.Compare(childFeature, featureVector[elementIndex - 1].Key) > 0)
                    {
                        var childComparedToCurrent = featureComparer.Compare(childFeature, featureVector[elementIndex].Key);

                        if (childComparedToCurrent <= 0)
                        {
                            var keyElementIndexOffset = childComparedToCurrent == 0 && childMagnitude >= featureVector[elementIndex].Value ? 1 : 0;

                            await foreach (var value in ExpandNode(childNode, elementIndex + keyElementIndexOffset))
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
            // NB: note that we need to filter the values to those keyed by clauses that are
            // actually subsumed by the query clause. The node values are the candidate set.
            await foreach (var value in node.GetKeyValuePairs().Where(kvp => clause.Subsumes(kvp.Key)).Select(kvp => kvp.Value))
            {
                yield return value;
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

    private IList<KeyValuePair<TFeature, int>> MakeOrderedFeatureVector(CNFClause clause)
    {
        return featureVectorSelector(clause).OrderBy(kvp => kvp.Key, featureComparer).ToList();
    }
}
