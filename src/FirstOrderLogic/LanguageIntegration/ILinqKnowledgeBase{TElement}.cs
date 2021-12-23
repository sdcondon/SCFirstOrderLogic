using System.Collections.Generic;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// A store of knowledge expressed as statements of propositional logic (in turn expressed as LINQ expressions).
    /// </summary>
    /// <typeparam name="TElement">
    /// The type that the sentences passed to this class refer to.
    /// </typeparam>
    /// <remarks>
    /// This interface is just a shorthand for <c>IKnowledgeBase&lt;IEnumerable&lt;TElement&gt;, TElement&gt;</c>. It is explicitly included
    /// because a particular type for TDomain is only needed when there are constants or ground predicates - which is by no means all cases.
    /// </remarks>
    public interface ILinqKnowledgeBase<TElement> : ILinqKnowledgeBase<IEnumerable<TElement>, TElement>
    {
    }
}
