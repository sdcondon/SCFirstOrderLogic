// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// <para>
    /// A store of knowledge expressed as statements of propositional logic (in turn expressed as LINQ expressions).
    /// </para>
    /// <para>
    /// This interface is just a shorthand for <see cref="ILinqKnowledgeBase{TDomain, TElement}"/>, where <c>TDomain</c>
    /// is <c>IEnumerable&lt;TElement&gt;</c>. It is explicitly included because a concrete type for TDomain is only
    /// needed when there are constants or ground predicates - which is by no means all cases.
    /// </para>
    /// </summary>
    /// <typeparam name="TElement">
    /// The type that the sentences passed to this class refer to.
    /// </typeparam>
    public interface ILinqKnowledgeBase<TElement> : ILinqKnowledgeBase<IEnumerable<TElement>, TElement>
    {
    }
}
