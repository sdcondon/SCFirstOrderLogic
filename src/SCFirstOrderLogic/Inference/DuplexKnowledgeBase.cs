#if FALSE
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Decorator knowledge base class that, when answering queries, will concurrently execute the query and the negation of the query
    /// at the same time. This allows for returning a negative result in a reasonable timeframe when a query is known to be false.
    /// <para/>
    /// No reference back to the source material for this one.
    /// </summary>
    public class DuplexKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBase innerKnowledgeBase;

        /// <summary>
        /// Initialises a new instance of the <see cref="DuplexKnowledgeBase"/> class.
        /// </summary>
        /// <param name="innerKnowledgeBase">The underlying knowledge base to use.</param>
        public DuplexKnowledgeBase(IKnowledgeBase innerKnowledgeBase)
        {
            this.innerKnowledgeBase = innerKnowledgeBase;
        }

        /// <inheritdoc/>
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default) => innerKnowledgeBase.TellAsync(sentence, cancellationToken);

        /// <inheritdoc/>
        async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence query, CancellationToken cancellationToken)
        {
            return await CreateQueryAsync(query, cancellationToken);
        }

        /// <summary>
        /// Initiates a new query against this knowledge base.
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation</param>
        /// <returns>A (task that returns a) new <see cref="DuplexKnowledgeBaseQuery"/> instance.</returns>
        public async Task<DuplexKnowledgeBaseQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            return await DuplexKnowledgeBaseQuery.CreateAsync(innerKnowledgeBase, query, cancellationToken);
        }

        public class DuplexKnowledgeBaseQuery : IQuery
        {
            private DuplexKnowledgeBaseQuery(IQuery positiveQuery, IQuery negativeQuery)
            {
                PositiveQuery = positiveQuery;
                NegativeQuery = negativeQuery;
            }

            /// <inheritdoc/>
            public bool IsComplete { get; private set; }

            /// <inheritdoc/>
            public bool Result { get; private set; }

            /// <summary>
            /// Gets the positive version of the query being carried out.
            /// </summary>
            public IQuery PositiveQuery { get; private set; }

            /// <summary>
            /// Gets the negative version of the query being carried out.
            /// </summary>
            public IQuery NegativeQuery { get; private set; }

            /// <inheritdoc/>
            public async Task NextStepAsync(CancellationToken cancellationToken = default)
            {
                if (IsComplete)
                {
                    throw new InvalidOperationException("Query is complete");
                }
                else
                {
                    // Because we expose the two queries publicly, its possible that a caller has
                    // executed them directly. These checks are just a very simple bit of robustness to that.
                    // There are of course a bunch of alternative ways of handling this (all the way up to 
                    // creating a readonly variant of query types), but this hits a sweet spot of simplicity
                    // and robustness.
                    if (PositiveQuery.IsComplete)
                    {
                        IsComplete = true;
                        Result = PositiveQuery.Result;
                    }
                    else if (NegativeQuery.IsComplete)
                    {
                        IsComplete = true;
                        Result = NegativeQuery.Result;
                    }
                }

                var nextPositiveStepExecution = PositiveQuery.NextStepAsync(cancellationToken);
                var nextNegativeStepExecution = NegativeQuery.NextStepAsync(cancellationToken);

                // ..could await the first one to finish, but meh..
                await nextPositiveStepExecution;
                if (PositiveQuery.IsComplete)
                {
                    // NB: we don't bother awaiting the negative one. Might not be done by the time we return.
                    // We own both queries, and disposal should them both away anyway - so not a big deal, probably..
                    // TODO-ZZZ: If we were feeling keen we could cancel it (by creating our own linked cancellation
                    // token above rather than just propogating the one we get).
                    IsComplete = true;
                    Result = PositiveQuery.Result;
                }
                else
                {
                    await nextNegativeStepExecution;
                    if (NegativeQuery.IsComplete)
                    {
                        IsComplete = true;
                        Result = !NegativeQuery.Result;
                    }
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                PositiveQuery.Dispose();
                NegativeQuery.Dispose();
            }

            internal static async Task<DuplexKnowledgeBaseQuery> CreateAsync(IKnowledgeBase innerKnowledgeBase, Sentence sentence, CancellationToken cancellationToken)
            {
                var positiveQueryCreation = innerKnowledgeBase.CreateQueryAsync(sentence, cancellationToken);
                var negativeQueryCreation = innerKnowledgeBase.CreateQueryAsync(new Negation(sentence), cancellationToken);
                return new DuplexKnowledgeBaseQuery(await positiveQueryCreation, await negativeQueryCreation);
            }
        }
    }
}
#endif