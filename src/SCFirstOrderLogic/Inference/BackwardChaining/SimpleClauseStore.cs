using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// Implementation of <see cref="IClauseStore"/> that just uses an in-memory dictionary (keyed by consequent symbol) to store known clauses.
    /// </summary>
    public class SimpleClauseStore : IClauseStore
    {
        private readonly Dictionary<object, HashSet<CNFDefiniteClause>> clausesByConsequentSymbol = new();

        /// <inheritdoc/>
        public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
        {
            if (!clausesByConsequentSymbol.TryGetValue(clause.Consequent.Symbol, out var clausesWithThisConsequentSymbol))
            {
                clausesWithThisConsequentSymbol = clausesByConsequentSymbol[clause.Consequent.Symbol] = new HashSet<CNFDefiniteClause>();
            }

            return Task.FromResult(clausesWithThisConsequentSymbol.Add(clause));
        }

#pragma warning disable CS1998 // async lacks await.. Could stick a Task.Yield in there, but not worth it.
        /// <inheritdoc/>
        public async IAsyncEnumerable<(CNFDefiniteClause Clause, VariableSubstitution Substitution)> GetClauseApplications(Predicate goal, VariableSubstitution constraints, CancellationToken cancellationToken = default)
        {
            if (clausesByConsequentSymbol.TryGetValue(goal.Symbol, out var clausesWithThisGoal))
            {
                foreach (var clause in clausesWithThisGoal)
                {
                    var restandardisedClause = clause.Restandardize();
                    var substitution = new VariableSubstitution(constraints);

                    if (LiteralUnifier.TryUpdate(restandardisedClause.Consequent, goal, substitution))
                    {
                        yield return (restandardisedClause, substitution);
                    }
                }
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var clauseList in clausesByConsequentSymbol.Values)
            {
                foreach (var clause in clauseList)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return clause;
                }
            }
        }
#pragma warning restore CS1998
    }
}
