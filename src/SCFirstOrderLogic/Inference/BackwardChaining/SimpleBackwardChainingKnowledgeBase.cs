using SCFirstOrderLogic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a (depth-first) backward chaining algorithm.
    /// <para/>
    /// This is a VERY basic implementation - essentially just what is found in figure 9.6 of "Artificial Intelligence: A Modern Approach".
    /// More sophisticated implementations - for now at least - are outside of the scope of this package.
    /// </summary>
    public class SimpleBackwardChainingKnowledgeBase : IKnowledgeBase
    {
        private readonly IClauseStore clauseStore;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimpleBackwardChainingKnowledgeBase"/> class.
        /// </summary>
        /// <param name="clauseStore">The object to use to store and look up clauses.</param>
        public SimpleBackwardChainingKnowledgeBase(IClauseStore clauseStore) => this.clauseStore = clauseStore;

        /// <inheritdoc />
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            // Normalize, then verify that the sentence consists only of definite clauses
            // before indexing ANY of them:
            var cnfSentence = sentence.ToCNF();

            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses", nameof(sentence));
            }

            // Store clauses in the clause store:
            foreach (var clause in cnfSentence.Clauses)
            {
                await clauseStore.AddAsync(new CNFDefiniteClause(clause), cancellationToken);
            }
        }

        /// <inheritdoc />
        async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken)
        {
            return await CreateQueryAsync(sentence, cancellationToken);
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task that returns an <see cref="SimpleBackwardChainingQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
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

            return Task.FromResult(new SimpleBackwardChainingQuery(p, clauseStore));
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <returns>An <see cref="SimpleBackwardChainingQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public SimpleBackwardChainingQuery CreateQuery(Sentence query)
        {
            return CreateQueryAsync(query).GetAwaiter().GetResult();
        }
    }
}
