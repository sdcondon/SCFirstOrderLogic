// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Interface for the types that describe elements of a term, for the purposes of keying the
    /// nodes of a discrimination tree. Instances of this interface are associated with each 
    /// non-root element of a discrimination tree.
    /// </summary>
    public interface IDiscriminationTreeElementInfo
    {
        /// <summary>
        /// Gets the number of child elements of the element described by this object.
        /// </summary>
        int ChildElementCount { get; }
    }
}
