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
        private readonly IReadOnlyDictionary<object, List<CNFClause>> clausesByConsequentSymbol;

        private IEnumerable<VariableSubstitution>? substitutions;

        internal SimpleBackwardChainingQuery(Predicate query, IReadOnlyDictionary<object, List<CNFClause>> clausesByConsequentSymbol)
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
            substitutions = FOL_BC_OR(query, new VariableSubstitution());
            return Task.FromResult(Result);
        }

        private IEnumerable<VariableSubstitution> FOL_BC_OR(Predicate goal, VariableSubstitution θ)
        {
            foreach (var clause in clausesByConsequentSymbol[goal.Symbol])
            {
                var lhs = clause.Literals.Where(l => l.IsNegated).Select(l => l.Predicate);
                var rhs = clause.Literals.Single(l => l.IsPositive);
                var unifier = new VariableSubstitution(θ);

                if (LiteralUnifier.TryUpdate(rhs, goal, unifier))
                {
                    foreach (var θ2 in FOL_BC_AND(lhs, unifier))
                    {
                        yield return θ2;
                    }
                }
            }
        }

        private IEnumerable<VariableSubstitution> FOL_BC_AND(IEnumerable<Predicate> goals, VariableSubstitution θ)
        {
            if (!goals.Any())
            {
                yield return θ;
            }
            else
            {
                var first = goals.First();
                var rest = goals.Skip(1);
                foreach (var θ2 in FOL_BC_OR(θ.ApplyTo(first).Predicate, θ))
                {
                    foreach (var θ3 in FOL_BC_AND(rest, θ2))
                    {
                        yield return θ3;
                    }
                }
            }
        }
    }
}
