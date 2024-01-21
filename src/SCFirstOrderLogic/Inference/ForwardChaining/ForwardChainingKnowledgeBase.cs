// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a breadth-first, incremental forward chaining algorithm.
    /// </summary>
    public class ForwardChainingKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBaseClauseStore clauseStore;

        /// <summary>
        /// Initialises a new instance of the <see cref="ForwardChainingKnowledgeBase"/> class.
        /// </summary>
        /// <param name="clauseStore">the clause store to use to store and look up clauses.</param>
        public ForwardChainingKnowledgeBase(IKnowledgeBaseClauseStore clauseStore) => this.clauseStore = clauseStore;

        /// <inheritdoc />
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            // Normalize, then verify that the sentence consists only of definite clauses
            // before indexing ANY of them:
            var cnfSentence = sentence.ToCNF();

            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge that can be expressed as definite clauses", nameof(sentence));
            }

            // Store clauses in the clause store:
            // NB: we go one-by-one rather than assuming the clause store is thread-safe.
            // This is perhaps a needless performance hit - we should perhaps be assuming thread-safe clause stores and using Parallel.ForEach.
            // Then again, sentences in general won't have many clauses in them - so the overhead may dominate (in non-IO bound cases at least)?
            // This is fine for now at least.
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
        /// <returns>A task that returns an <see cref="ForwardChainingQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public async Task<ForwardChainingQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            if (query is not Predicate p)
            {
                throw new ArgumentException("This knowledge base supports only queries that are predicates");
            }

            // Doesn't hurt to not standardise here - wont clash because all of the KB rules *are* standardised
            // (assuming the identifiers of the variables in the query don't have weird equality rules)..
            // ..and in any case our standardisation logic assumes all variables to be quantified, otherwise it crashes..

            return new ForwardChainingQuery(p, await clauseStore.CreateQueryStoreAsync(cancellationToken));
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <returns>An <see cref="ForwardChainingQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public ForwardChainingQuery CreateQuery(Sentence query)
        {
            return CreateQueryAsync(query).GetAwaiter().GetResult();
        }
    }
}
