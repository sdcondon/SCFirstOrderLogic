using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Query implementation used by <see cref="SimpleBackwardChainingKnowledgeBase"/>.
    /// </summary>
    public class SimpleBackwardChainingQuery : IQuery
    {
        private readonly Predicate query;
        private readonly IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol;

        private IEnumerable<VariableSubstitution>? substitutions;

        internal SimpleBackwardChainingQuery(Predicate query, IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol)
        {
            this.query = query;
            this.clausesByConsequentSymbol = clausesByConsequentSymbol;
        }

        /// <inheritdoc />
        public bool IsComplete => substitutions != null;

        /// <inheritdoc />
        public bool Result => substitutions?.Any() ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets the set of variable substitutions that can be made to satisfy the query.
        /// Result will be empty if and only if the query returned a negative result.
        /// </summary>
        public IEnumerable<VariableSubstitution> Substitutions => substitutions ?? throw new InvalidOperationException("Query is not yet complete");

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            substitutions = VisitPredicate(query, new VariableSubstitution());
            return Task.Run(() => Result, cancellationToken);
        }

        /// <summary>
        /// Attempt to verify the truth of a predicate by (recursively) attempting to verify the truth of at least one of the clauses for which it is a consequent -
        /// making only variable substitutions that do not conflict with substitutions already made.
        /// </summary>
        /// <param name="goal">The predicate in question.</param>
        /// <param name="unifier">The current unifier.</param>
        /// <returns></returns>
        private IEnumerable<VariableSubstitution> VisitPredicate(Predicate goal, VariableSubstitution unifier)
        {
            foreach (var clause in clausesByConsequentSymbol[goal.Symbol])
            {
                var updatedUnifier = new VariableSubstitution(unifier);

                if (LiteralUnifier.TryUpdate(clause.Consequent, goal, updatedUnifier))
                {
                    foreach (var result in VisitConjuncts(clause.Conjuncts, updatedUnifier))
                    {
                        yield return result;
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to verify the truth of all a set of conjoined predicates in a particular clause by (recursively) attempting to verify each conjunct -
        /// making only variable substitutions that do not conflict with substitutions already made.
        /// </summary>
        /// <param name="conjuncts">The conjuncts in question.</param>
        /// <param name="unifier">The current unifier.</param>
        /// <returns></returns>
        private IEnumerable<VariableSubstitution> VisitConjuncts(IEnumerable<Predicate> conjuncts, VariableSubstitution unifier)
        {
            if (!conjuncts.Any())
            {
                yield return unifier;
            }
            else
            {
                foreach (var firstPredicateResult in VisitPredicate(unifier.ApplyTo(conjuncts.First()).Predicate, unifier))
                {
                    foreach (var result in VisitConjuncts(conjuncts.Skip(1), firstPredicateResult))
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}
