// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
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
/// Feature vector indexing is an non-perfect indexing method for clause subsumption.
/// That is, feature vector indices can be used to store clauses in such a way that we can quickly look up the stored clauses that subsume or are subsumed by a query clause.
/// </para>
/// </summary>
/// <typeparam name="TFeature">The type of the keys of the feature vectors.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each stored clause.</typeparam>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
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
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> elementComparer)
        : this(featureVectorSelector, elementComparer, new FeatureVectorIndexDictionaryNode<TFeature, TValue>(), EmptyElements)
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
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> elementComparer,
        IFeatureVectorIndexNode<TFeature, TValue> root)
        : this(featureVectorSelector, elementComparer, root, EmptyElements)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureVectorIndex{TFeature,TValue}"/> class with a new 
    /// <see cref="FeatureVectorIndexDictionaryNode{TFeature,TValue}"/> root node and some (additional) initial
    /// content.
    /// </summary>
    /// <param name="featureVectorSelector">The delegate to use to retrieve the feature vector for any given clause.</param>
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> elementComparer,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
        : this(featureVectorSelector, elementComparer, new FeatureVectorIndexDictionaryNode<TFeature, TValue>(), content)
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
    /// <param name="elementComparer">
    /// The comparer to use to determine the ordering of features when adding to the index and performing
    /// queries. NB: For correct behaviour, the index must be able to unambiguously order the features (i.e. keys)
    /// of a feature vector. As such, this comparer must only return zero for equal features (and of course 
    /// duplicates shouldn't occur in any given vector).
    /// </param>
    /// <param name="root">The root node of the tree.</param>
    /// <param name="content">The (additional) content to be added to the tree (beyond any already attached to the provided root node).</param>
    public FeatureVectorIndex(
        Func<CNFClause, IEnumerable<KeyValuePair<TFeature, int>>> featureVectorSelector,
        IComparer<TFeature> elementComparer, IFeatureVectorIndexNode<TFeature, TValue> root,
        IEnumerable<KeyValuePair<CNFClause, TValue>> content)
    {
        ArgumentNullException.ThrowIfNull(featureVectorSelector);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(elementComparer);
        ArgumentNullException.ThrowIfNull(content);

        this.featureVectorSelector = featureVectorSelector;
        this.root = root;
        this.featureComparer = elementComparer;

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
        foreach (var element in MakeOrderedFeatureVector(key))
        {
            currentNode = currentNode.GetOrAddChildNode(element);
        }

        currentNode.AddValue(value);
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

        bool ExpandNode(IFeatureVectorIndexNode<TFeature, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < featureVector.Count)
            {
                var element = featureVector[keyElementIndex];

                if (!node.Children.TryGetValue(element, out var childNode) || !ExpandNode(childNode, keyElementIndex + 1))
                {
                    return false;
                }

                if (childNode.Children.Count == 0 && !childNode.HasValue)
                {
                    node.DeleteChild(element);
                }

                return true;
            }
            else
            {
                if (!node.HasValue)
                {
                    return false;
                }

                node.RemoveValue();
                return true;
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
        foreach (var keyElement in MakeOrderedFeatureVector(key))
        {
            if (currentNode.Children.TryGetValue(keyElement, out var childNode))
            {
                currentNode = childNode;
            }
            else
            {
                value = default;
                return false;
            }
        }

        if (currentNode.HasValue)
        {
            value = currentNode.Value;
            return true;
        }

        value = default;
        return false;
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

        IEnumerable<TValue> ExpandNode(IFeatureVectorIndexNode<TFeature, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < featureVector.Count)
            {
                if (node.Children.TryGetValue(featureVector[keyElementIndex], out var childNode))
                {
                    foreach (var value in ExpandNode(childNode, keyElementIndex + 1))
                    {
                        yield return value;
                    }
                }

                foreach (var value in ExpandNode(node, keyElementIndex + 1))
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
    /// Retrieves the values associated with each stored clause that is subsumed by a given set.
    /// </summary>
    /// <param name="clause">The values associated with the stored clauses that are subsumed by this clause will be retrieved.</param>
    /// <returns>An enumerable of the values associated with each stored clause that is subsumed by the given clause.</returns>
    public IEnumerable<TValue> GetSubsumed(CNFClause clause)
    {
        ArgumentNullException.ThrowIfNull(clause);

        var featureVector = MakeOrderedFeatureVector(clause);

        return ExpandNode(root, 0);

        IEnumerable<TValue> ExpandNode(IFeatureVectorIndexNode<TFeature, TValue> node, int keyElementIndex)
        {
            if (keyElementIndex < featureVector.Count)
            {
                foreach (var (childKeyElement, childNode) in node.Children)
                {
                    if (keyElementIndex == 0 || featureComparer.Compare(childKeyElement, featureVector[keyElementIndex - 1]) > 0)
                    {
                        var childComparedToCurrent = featureComparer.Compare(childKeyElement, featureVector[keyElementIndex]);

                        if (childComparedToCurrent <= 0)
                        {
                            var keyElementIndexOffset = childComparedToCurrent == 0 ? 1 : 0;

                            foreach (var value in ExpandNode(childNode, keyElementIndex + keyElementIndexOffset))
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
            if (node.HasValue)
            {
                yield return node.Value;
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
