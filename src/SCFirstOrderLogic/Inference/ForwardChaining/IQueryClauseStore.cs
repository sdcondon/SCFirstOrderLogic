using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// Sub-type of <see cref="IClauseStore"/> for types intended for storing the intermediate clauses of a particular query.
    /// Notably, is <see cref="IDisposable"/> so that any unmanaged resources can be promptly tidied away once the query is complete.
    /// </summary>
    public interface IQueryClauseStore : IClauseStore, IDisposable
    {
        /// <summary>
        /// Gets all of the rules whose conjuncts contain at least one that unifies with at least one of a given set of facts.
        /// </summary>
        /// <param name="facts">The facts.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>An enumerable of applicable rules.</returns>
        IAsyncEnumerable<CNFDefiniteClause> GetApplicableRules(IEnumerable<Predicate> facts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns all possible resolutions of a given clause with some clause in the store.
        /// </summary>
        /// <param name="fact">The fact to check for.</param>
        /// <param name="substitution">The variable values that must be respected.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns></returns>
        IAsyncEnumerable<(Predicate knownFact, VariableSubstitution unifier)> MatchWithKnownFacts(Predicate fact, VariableSubstitution substitution, CancellationToken cancellationToken = default);
    }
}
