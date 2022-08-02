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
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a backward chaining algorithm. This one is implemented as close as possible to the
    /// implementation in figure 9.6 of "Artificial Intelligence: A Modern Approach" - for reference purposes.
    /// </summary>
    public class AltBackwardChainingKnowledgeBase_FromAIaMA : IKnowledgeBase
    {
        private readonly Dictionary<object, List<CNFClause>> clausesByConsequentSymbol = new ();

        /// <inheritdoc />
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            var cnfSentence = new CNFSentence(sentence);

            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses", nameof(sentence));
            }

            foreach (var clause in cnfSentence.Clauses)
            {
                var consequentSymbol = clause.Literals.Single(l => l.IsPositive).Predicate.Symbol;

                if (!clausesByConsequentSymbol.TryGetValue(consequentSymbol, out var clausesWithThisConsequentSymbol))
                {
                    clausesWithThisConsequentSymbol = clausesByConsequentSymbol[consequentSymbol] = new List<CNFClause>();
                }

                clausesWithThisConsequentSymbol.Add(clause);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken)
        {
            return await CreateQueryAsync(sentence, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Query> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            if (query is not Predicate p)
            {
                throw new ArgumentException("This knowledge base supports only queries that are predicates");
            }

            // Doesn't hurt to not standardise here - wont clash because all of the KB rules *are* standardised
            // (assuming the symbols in the query don't have weird equality rules)..
            // ..and in any case our standardisation logic assumes all variables to be quantified, otherwise it crashes..
            //var standardisation = new VariableStandardisation(query);
            //p = (Predicate)standardisation.ApplyTo(p);

            return Task.FromResult(new Query(p, clausesByConsequentSymbol));
        }

        /// <summary>
        /// Query implementation used by <see cref="AltBackwardChainingKnowledgeBase_FromAIaMA"/>.
        /// </summary>
        public class Query : IQuery
        {
            private readonly Predicate query;
            private readonly IReadOnlyDictionary<object, List<CNFClause>> clausesByConsequentSymbol;

            private IEnumerable<VariableSubstitution>? substitutions;

            internal Query(Predicate query, IReadOnlyDictionary<object, List<CNFClause>> clausesByConsequentSymbol)
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
}
