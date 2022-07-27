using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// Query implementation used by <see cref="SimpleBackwardChainingKnowledgeBase"/>.
    /// </summary>
    public class SimpleBackwardChainingQuery : IQuery
    {
        private readonly Predicate query;
        private readonly IReadOnlyDictionary<object, List<CNFClause>> clausesByConsequentSymbol;

        internal SimpleBackwardChainingQuery(Predicate query, IReadOnlyDictionary<object, List<CNFClause>> clausesByConsequentSymbol)
        {
            this.query = query;
            this.clausesByConsequentSymbol = clausesByConsequentSymbol;
        }

        /// <inheritdoc />
        public bool IsComplete { get; private set; }

        /// <inheritdoc />
        public bool Result { get; private set; }

        public IEnumerable<VariableSubstitution> Substitutions { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Substitutions = FOL_BC_OR(query, new VariableSubstitution());

            Result = Substitutions.Count() > 0;
            IsComplete = true;

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
