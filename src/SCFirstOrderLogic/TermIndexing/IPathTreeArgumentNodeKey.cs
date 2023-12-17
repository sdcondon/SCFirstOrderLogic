// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Interface for types that describe elements of a term, for the purposes of storage in a path tree.
    /// Instances of this type are used as the key of each child of a <see cref="IPathTreeParameterNode{TValue}"/>.
    /// </summary>
    public interface IPathTreeArgumentNodeKey
    {
        /// <summary>
        /// Gets the number of child elements of the element described by this object.
        /// </summary>
        int ChildElementCount { get; }
    }

    /// <summary>
    /// Information about a function, for storage against a node of a <see cref="PathTree{TValue}"/>.
    /// </summary>
    /// <param name="Identifier">The identifier of the represented function.</param>
    /// <param name="ArgumentCount">The number of arguments of the represented function.</param>
    public sealed record PathTreeFunctionNodeKey(object Identifier, int ArgumentCount) : IPathTreeArgumentNodeKey
    {
        /// <inheritdoc/>
        public int ChildElementCount => ArgumentCount;
    }

    /// <summary>
    /// Information about a constant, for storage against a node of a <see cref="PathTree{TValue}"/>.
    /// </summary>
    /// <param name="Identifier">The identifier of the represented constant.</param>
    public sealed record PathTreeConstantNodeKey(object Identifier) : IPathTreeArgumentNodeKey
    {
        /// <inheritdoc/>
        public int ChildElementCount => 0;
    }

    /// <summary>
    /// Information about a variable, for storage against a node of a <see cref="PathTree{TValue}"/>.
    /// </summary>
    /// <param name="Ordinal">
    /// The ordinal of the represented variable - that is, the index of its position in a list of variables that
    /// exist in the term, in the order in which they are first encountered by a depth-first traversal.
    /// </param>
    public sealed record PathTreeVariableNodeKey(int Ordinal) : IPathTreeArgumentNodeKey
    {
        /// <inheritdoc/>
        public int ChildElementCount => 0;
    }
}
