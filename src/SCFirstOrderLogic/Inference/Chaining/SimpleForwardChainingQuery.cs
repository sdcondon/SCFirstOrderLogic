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
    /// Query implementation used by <see cref="SimpleForwardChainingKnowledgeBase"/>.
    /// </summary>
    public sealed class SimpleForwardChainingQuery : IQuery
    {
        private readonly Predicate α;
        private readonly List<CNFDefiniteClause> kb;

        private bool? result;
        private VariableSubstitution? substitution;

        internal SimpleForwardChainingQuery(Predicate α, List<CNFDefiniteClause> clauses)
        {
            this.α = α;
            this.kb = new List<CNFDefiniteClause>(clauses);
        }

        /// <inheritdoc />
        public bool IsComplete => result != null;

        /// <inheritdoc />
        public bool Result => result ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets the variable substitution that is carried out to resolve the query. Throws an exception if the query is not complete or returned a negative result.
        /// </summary>
        public VariableSubstitution Substitution
        {
            get
            {
                if (!IsComplete)
                {
                    throw new InvalidOperationException("Query is not yet complete");
                }

                if (!Result)
                {
                    throw new InvalidOperationException("Query returned a negative result");
                }

                return substitution!;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var @new = new List<CNFDefiniteClause>();

            do
            {
                @new.Clear();

                foreach (var rule in kb)
                {
                    // NB: we don't need the variable standardisation line from the book because that's already
                    // happened as part of the conversion to CNF that is carried out when the KB is told things.
                    // So all we do is call the consequent 'q' to match the book listing:
                    var q = rule.Consequent;

                    foreach (var θ in MatchWithKnownFacts(rule))
                    {
                        var qDash = θ.ApplyTo(q).Predicate;
                        var qDashAsClause = new CNFDefiniteClause(qDash); // Worth looking into a bit more type fluidity at some point..

                        if (!qDashAsClause.UnifiesWithAnyOf(kb) && !qDashAsClause.UnifiesWithAnyOf(@new))
                        {
                            @new.Add(qDashAsClause);

                            if (LiteralUnifier.TryCreate(qDash, α, out var φ))
                            {
                                result = true;
                                substitution = φ;
                                return Task.FromResult(true);
                            }
                        }
                    }
                }

                kb.AddRange(@new);
            }
            while (@new.Count > 0);

            result = false;
            return Task.FromResult(false);
        }

        private IEnumerable<VariableSubstitution> MatchWithKnownFacts(CNFDefiniteClause clause)
        {
            // NB: no specific conjunct ordering here - just look at them in the order they happen to fall.
            // In a production scenario, we'd at least TRY to order the conjuncts in a way that minimises
            // the amount of work we have to do.
            return MatchWithKnownFacts(clause.Conjuncts, new VariableSubstitution());
        }

        // I'm not a huge fan of recursion when trying to write reference code but I'll admit it is handy here.. May revisit this..
        private IEnumerable<VariableSubstitution> MatchWithKnownFacts(IEnumerable<Predicate> conjuncts, VariableSubstitution unifier)
        {
            if (!conjuncts.Any())
            {
                yield return unifier;
            }
            else
            {
                // Here we just iterate through ALL known predicates trying to find something that unifies with the first conjunct.
                // We'd use an index here in anything approaching a production scenario:
                foreach (var knownClause in kb.Where(k => k.IsUnitClause))
                {
                    var updatedUnifier = new VariableSubstitution(unifier);

                    if (LiteralUnifier.TryUpdate(knownClause.Consequent, conjuncts.First(), updatedUnifier))
                    {
                        foreach (var substitution in MatchWithKnownFacts(conjuncts.Skip(1), updatedUnifier))
                        {
                            yield return substitution;
                        }
                    }
                }
            }
        }
    }
}
