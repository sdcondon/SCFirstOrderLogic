// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// <para>
/// An implementation of a feature vector index for <see cref="CNFClause"/>s.
/// </para>
/// <para>
/// Feature vector indexing (in this context, at least) is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
public class FeatureVectorIndex<TFeature, TValue>
    where TFeature : notnull
{
    private static readonly IEnumerable<KeyValuePair<CNFClause, TValue>> EmptyElements = Enumerable.Empty<KeyValuePair<CNFClause, TValue>>();

    private readonly Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector;
    private readonly IFeatureVectorIndexNode<TFeature, TValue> root;
    private readonly IComparer<TFeature> featureComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and no initial content, that
    /// uses the default comparer of the key element type to determine the ordering of elements in the
    /// tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector)
        : this(featureVectorSelector, Comparer<TFeature>.Default, new FeatureVectorIndexDictionaryNode<TFeature, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and no initial content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="featureComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer)
        : this(featureVectorSelector, featureComparer, new FeatureVectorIndexDictionaryNode<TFeature, TValue>(), EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a specified
    /// root node and no (additional) initial content, that uses the default comparer of the key element
    /// type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<TFeature, TValue> root)
        : this(featureVectorSelector, Comparer<TFeature>.Default, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some initial content,
    /// that uses the default comparer of the key element type to determine the ordering of elements
    /// in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="content">The initial content to be added to the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, Comparer<TFeature>.Default, new FeatureVectorIndexDictionaryNode<TFeature, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a 
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
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IFeatureVectorIndexNode<TFeature, TValue> root)
        : this(featureVectorSelector, featureComparer, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some (additional) initial
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
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, featureComparer, new FeatureVectorIndexDictionaryNode<TFeature, TValue>(), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a 
    /// specified root node and some (additional) initial content, that uses the default comparer
    /// of the key element type to determine the ordering of elements in the tree.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IFeatureVectorIndexNode<TFeature, TValue> root,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, Comparer<TFeature>.Default, root, content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a 
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
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> featureComparer,
        IFeatureVectorIndexNode<TFeature, TValue> root,
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
            Add(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Adds a clause and associated value to the index.
    /// </summary>
    /// <param name="key">The clause to add.</param>
    /// <param name="value">The value to associate with the clause.</param>
    public void Add(CNFClause key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var currentNode = root;
        foreach (var component in MakeOrderedFeatureVector(key))
        {
            currentNode = currentNode.GetOrAddChild(component);
        }

        currentNode.AddValue(key, value);
    }

    /// <summary>
    /// Removes a clause from the index.
    /// </summary>
    /// <param name="key">The clause to remove.</param>
    /// <returns>A value indicating whether the clause was present prior to this operation.</returns>
    public bool Remove(CNFClause key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var featureVector = MakeOrderedFeatureVector(key);

        return ExpandNode(root, 0);

        bool ExpandNode(IFeatureVectorIndexNode<TFeature, TValue> node, int vectorComponentIndex)
        {
            if (vectorComponentIndex < featureVector.Count)
            {
                var element = featureVector[vectorComponentIndex];

                if (!node.Children.TryGetValue(element, out var childNode) || !ExpandNode(childNode, vectorComponentIndex + 1))
                {
                    return false;
                }

                if (childNode.Children.Count == 0 && !childNode.HasValues)
                {
                    node.DeleteChild(element);
                }

                return true;
            }
            else
            {
                return node.RemoveValue(key);
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve the value associated with a clause, matched exactly.
    /// </summary>
    /// <param name="key">The clause to retrieve the associated value of.</param>
    /// <param name="value">Will be populated with the retrieved value.</param>
    /// <returns>True if and only if a value was successfully retrieved.</returns>
    public bool TryGet(CNFClause key, [MaybeNullWhen(false)] out TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var currentNode = root;
        foreach (var vectorComponent in MakeOrderedFeatureVector(key))
        {
            if (currentNode.Children.TryGetValue(vectorComponent, out var childNode))
            {
                currentNode = childNode;
            }
            else
            {
                value = default;
                return false;
            }
        }

        return currentNode.TryGetValue(key, out value);
    }

    /// <summary>
    /// Retrieves the values associated with each stored clause that subsumes a given clause.
    /// </summary>
    /// <param name="clause">The values associated with the stored clauses that subsume this clause will be retrieved.</param>
    /// <returns>An enumerable of the values associated with each stored clause that subsumes the given clause.</returns>
    public IEnumerable<TValue> GetSubsuming(CNFClause clause)
    {
        ArgumentNullException.ThrowIfNull(clause);

        var featureVector = MakeOrderedFeatureVector(clause);

        return ExpandNode(root, 0);

        // NB: subsumed clauses will have equal or higher vector elements. So subsuming clauses will have equal or lower vector elements.
        // We allow zero-valued elements to be omitted from the vectors (so that we don't have to know what identifiers are possible ahead of time).
        // This makes the logic here a little similar to what you'd find in a set trie when querying for subsets.
        IEnumerable<TValue> ExpandNode(IFeatureVectorIndexNode<TFeature, TValue> node, int vectorComponentIndex)
        {
            if (vectorComponentIndex < featureVector.Count)
            {
                // If matching feature with lower value, then recurse
                // TODO: can be made more efficient once node children are ordered
                foreach (var ((childFeature, childMagnitude), childNode) in node.Children)
                {
                    if (childFeature.Equals(featureVector[vectorComponentIndex].Key) && childMagnitude <= featureVector[vectorComponentIndex].Value)
                    {
                        foreach (var value in ExpandNode(childNode, vectorComponentIndex + 1))
                        {
                            yield return value;
                        }
                    }
                }

                // Matching feature might not be there at all in stored clauses, which means
                // it has a value of zero (so can't preclude subsumption) - so also just skip the current key element.
                foreach (var value in ExpandNode(node, vectorComponentIndex + 1))
                {
                    yield return value;
                }
            }
            else
            {
                // NB: note that we need to filter the values to those keyed by the clauses that
                // actually subsume the query clause. The values of the node are the candidate set.
                foreach (var value in node.GetSubsumingValues(clause))
                {
                    yield return value;
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the values associated with each stored clause that is subsumed by a given set.
    /// </summary>
    /// <param name="clause">The values associated with the stored clauses that are subsumed by this clause will be retrieved.</param>
    /// <returns>An enumerable of the values associated with each stored clause that is subsumed by the given clause.</returns>
    public IEnumerable<TValue> GetSubsumed(CNFClause clause)
    {
        ArgumentNullException.ThrowIfNull(clause);

        var featureVector = MakeOrderedFeatureVector(clause);

        return ExpandNode(root, 0);

        // NB: subsumed clauses will have equal or higher vector elements.
        // We allow zero-valued elements to be omitted from the vectors (so that we don't have to know what identifiers are possible ahead of time).
        // This makes the logic here a little similar to what you'd find in a set trie when querying for supersets.
        IEnumerable<TValue> ExpandNode(IFeatureVectorIndexNode<TFeature, TValue> node, int vectorComponentIndex)
        {
            if (vectorComponentIndex < featureVector.Count)
            {
                foreach (var ((childFeature, childMagnitude), childNode) in node.Children)
                {
                    // todo: is this right? or do we need by feature AND magnitude here?
                    if (vectorComponentIndex == 0 || featureComparer.Compare(childFeature, featureVector[vectorComponentIndex - 1].Key) > 0)
                    {
                        var childComparedToCurrent = featureComparer.Compare(childFeature, featureVector[vectorComponentIndex].Key);

                        if (childComparedToCurrent <= 0)
                        {
                            var queryComponentIndexOffset = childComparedToCurrent == 0 && childMagnitude >= featureVector[vectorComponentIndex].Value ? 1 : 0;

                            foreach (var value in ExpandNode(childNode, vectorComponentIndex + queryComponentIndexOffset))
                            {
                                yield return value;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var value in GetAllDescendentValues(node))
                {
                    yield return value;
                }
            }
        }

        IEnumerable<TValue> GetAllDescendentValues(IFeatureVectorIndexNode<TFeature, TValue> node)
        {
            // NB: note that we need to filter the values to those keyed by clauses that are
            // actually subsumed by the query clause. The values of the node are the candidate set.
            foreach (var value in node.GetSubsumedValues(clause))
            {
                yield return value;
            }

            foreach (var childNode in node.Children.Values)
            {
                foreach (var value in GetAllDescendentValues(childNode))
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
