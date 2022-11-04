using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClauseStore"/> class.
        /// </summary>
        public SimpleClauseStore() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClauseStore"/> class that is pre-populated with some knowledge.
        /// <para/>
        /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
        /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
        /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
        /// </summary>
        /// <param name="sentences">The initial content of the store.</param>
        public SimpleClauseStore(IEnumerable<Sentence> sentences)
        {
            foreach (var sentence in sentences)
            {
                foreach (var clause in sentence.ToCNF().Clauses)
                {
                    if (!clause.IsDefiniteClause)
                    {
                        throw new ArgumentException($"All forward chaining knowledge must be expressable as definite clauses. The normalisation of {sentence} includes {clause}, which is not a definite clause");
                    }

                    AddAsync(new CNFDefiniteClause(clause)).GetAwaiter().GetResult();
                }
            }
        }

        /// <inheritdoc/>
        public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
        {
            if (!clausesByConsequentSymbol.TryGetValue(clause.Consequent.Symbol, out var clausesWithThisConsequentSymbol))
            {
                clausesWithThisConsequentSymbol = clausesByConsequentSymbol[clause.Consequent.Symbol] = new HashSet<CNFDefiniteClause>();
            }

            return Task.FromResult(clausesWithThisConsequentSymbol.Add(clause));
        }

#pragma warning disable CS1998 // async lacks await.. Could add await Task.Yield() to silence this, but it is not worth the overhead.
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

        /// <inheritdoc/>
        public async IAsyncEnumerable<(CNFDefiniteClause Clause, VariableSubstitution Substitution)> GetClauseApplications(
            Predicate goal,
            VariableSubstitution constraints,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
#pragma warning restore CS1998
    }
}
