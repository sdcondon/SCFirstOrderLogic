using SCFirstOrderLogic.Inference.Resolution;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a backward chaining algorithm.
    /// <para/>
    /// For now this is VERY basic - just an implementation of what is found in figure 9.6 of "Artificial Intelligence: A Modern Approach".
    /// Would be nice to at least bring it up to comparability with the <see cref="SimpleResolutionKnowledgeBase"/> implementation - 
    /// with explanation methods and perhaps a little configurability.
    /// </summary>
    public class SimpleBackwardChainingKnowledgeBase : IKnowledgeBase
    {
        private readonly Dictionary<object, List<CNFClause>> clausesByConsequentSymbol = new();

        /// <inheritdoc />
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            // Normalize, then verify that the sentence consists only of definite clauses:
            var cnfSentence = new CNFSentence(sentence);

            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses", nameof(sentence));
            }

            // Store clauses just in memory, but indexed by their consequent symbol:
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
        public Task<SimpleBackwardChainingQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            if (query is not Predicate p)
            {
                throw new ArgumentException("This knowledge base supports only queries that are predicates");
            }

            // Doesn't hurt to not standardise variables here - wont clash because all of the KB rules *are* standardised
            // (assuming the symbols in the query don't have weird equality rules)..
            // ..and in any case our standardisation logic (is within CNFConversion for the moment and) assumes all variables to be quantified..
            //var standardisation = new VariableStandardisation(query);
            //p = (Predicate)standardisation.ApplyTo(p);

            return Task.FromResult(new SimpleBackwardChainingQuery(p, clausesByConsequentSymbol));
        }
    }
}
