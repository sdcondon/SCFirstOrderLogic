// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.TermIndexing;

/// <summary>
/// Interface for nodes that represent a particular value taken by a particular function parameter
/// (or, for children of the root node, a value of the whole term) that is present in at least
/// one of the terms stored in the path tree.
/// </summary>
public interface IAsyncPathTreeArgumentNode<TValue>
{
    /// <summary>
    /// Gets nodes representing each parameter of an internal node. Empty for leaf nodes.
    /// </summary>
    IAsyncEnumerable<IAsyncPathTreeParameterNode<TValue>> GetChildren();

    /// <summary>
    /// 
    /// </summary>
    ValueTask<IAsyncPathTreeParameterNode<TValue>> GetChildAsync(int index);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    ValueTask<IAsyncPathTreeParameterNode<TValue>> GetOrAddChildAsync(int index);

    /// <summary>
    /// Gets the values attached to a leaf node. Throws an exception for internal nodes.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<Term, TValue>> GetValues();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="term"></param>
    /// <param name="value"></param>
    ValueTask AddValueAsync(Term term, TValue value);
}
