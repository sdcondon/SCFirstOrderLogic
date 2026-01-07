// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Threading;
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
    ValueTask<IAsyncPathTreeParameterNode<TValue>> GetChildAsync(int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    ValueTask<IAsyncPathTreeParameterNode<TValue>> GetOrAddChildAsync(int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the values attached to a leaf node. Throws an exception for internal nodes.
    /// </summary>
    IAsyncEnumerable<KeyValuePair<Term, TValue>> GetValues();

    /// <summary>
    /// 
    /// </summary>
    ValueTask AddValueAsync(Term term, TValue value, CancellationToken cancellationToken = default);
}
