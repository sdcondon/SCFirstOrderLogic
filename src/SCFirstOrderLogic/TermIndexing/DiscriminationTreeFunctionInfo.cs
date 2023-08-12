// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Information about a function, for storage against a node of a discrimination tree.
    /// </summary>
    /// <param name="Identifier">The identifier of the represented function.</param>
    /// <param name="ArgumentCount">The number of arguments of the represented function.</param>
    public sealed record DiscriminationTreeFunctionInfo(object Identifier, int ArgumentCount) : IDiscriminationTreeElementInfo
    {
        /// <inheritdoc/>
        public int ChildElementCount => ArgumentCount;
    }
}
