// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// Interface for nodes that represent a particular value taken by a particular function parameter
/// (or, for children of the root node, a value of the whole term) that is present in at least
/// one of the terms stored in the path tree.
/// </summary>
public interface IPathTreeArgumentNode<TValue>
{
    /// <summary>
    /// Gets nodes representing each parameter of an internal node. Empty for leaf nodes.
    /// </summary>
    IReadOnlyList<IPathTreeParameterNode<TValue>> Children { get; }

    /// <summary>
    /// Gets the values attached to a leaf node. Throws an exception for internal nodes.
    /// </summary>
    IEnumerable<KeyValuePair<Term, TValue>> Values { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IPathTreeParameterNode<TValue> GetOrAddChild(int index);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="term"></param>
    /// <param name="value"></param>
    void AddValue(Term term, TValue value);
}
