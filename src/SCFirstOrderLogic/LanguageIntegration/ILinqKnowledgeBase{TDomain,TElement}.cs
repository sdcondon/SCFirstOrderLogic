using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// A store of knowledge expressed as statements of propositional logic (in turn expressed as LINQ expressions acting on the domain represented as an <see cref="IEnumerable{T}"/>).
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain. The domain must be modelled as an <see cref="IEnumerable{T}"/> of <see cref="TElement"/>.</typeparam>
    /// <typeparam name="TElement">The type that the sentences passed to this class refer to.</typeparam>
    /// <remarks>
    /// Not 100% convinced by the need for this interface, given that I'm not sure of the need for any implementation other than the one provided.
    /// There may be some reason for keeping the Expressions around to e.g. help out with AskVars type stuff though. So keeping for now. 
    /// </remarks>
    public interface ILinqKnowledgeBase<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Inform the knowledge base that a given expression acting on the domain can be assumed to evaluate to true when answering queries.
        /// </summary>
        /// <param name="sentence">The expression that can be assumed to evaluate to true when answering queries.</param>
        public Task TellAsync(Expression<Predicate<TDomain>> sentence, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ask the knowledge base whether a given expression acting on the domain must be true, given what it knows.
        /// </summary>
        /// <param name="query">The sentence to ask about.</param>
        /// <returns>True if the sentence is known to be true, false if it is known to be false or cannot be determined.</returns>
        public Task<bool> AskAsync(Expression<Predicate<TDomain>> query, CancellationToken cancellationToken = default);

        //// NB: No AskVars just yet.. Still half-hoping that implementing IQueryable with support
        //// for Where is doable (though the more I think about it the less feasible it seems)
    }
}
