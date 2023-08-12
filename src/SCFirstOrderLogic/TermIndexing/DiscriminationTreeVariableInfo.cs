// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Information about a variable, for storage against a node of a discrimination tree.
    /// </summary>
    /// <param name="Ordinal">
    /// The ordinal of the represented variable - that is, the index of its position in a list of variables that
    /// exist in the term, in the order in which they are first encountered by a depth-first traversal.
    /// </param>
    public sealed record DiscriminationTreeVariableInfo(int Ordinal) : IDiscriminationTreeElementInfo
    {
        /// <inheritdoc/>
        public int ChildElementCount => 0;
    }
}
