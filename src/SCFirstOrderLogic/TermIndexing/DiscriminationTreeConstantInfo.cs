// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Information about a constant, for storage against a node of a discrimination tree.
    /// </summary>
    /// <param name="Identifier">The identifier of the represented constant.</param>
    public sealed record DiscriminationTreeConstantInfo(object Identifier) : IDiscriminationTreeElementInfo
    {
        /// <inheritdoc/>
        public int ChildElementCount => 0;
    }
}
