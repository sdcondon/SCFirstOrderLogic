// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Interface for types that describe elements of a term, for the purposes of storage in a discrimination tree.
    /// Instances of this type are used as the key for each child of a <see cref="IDiscriminationTreeNode{TValue}"/>.
    /// </summary>
    // TODO-BREAKING-V6: "NodeKey" is a better suffix than "ElementInfo" - because it is more specific (and shorter).
    public interface IDiscriminationTreeElementInfo
    {
        /// <summary>
        /// Gets the number of child elements of the element described by this object.
        /// </summary>
        int ChildElementCount { get; }
    }

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
