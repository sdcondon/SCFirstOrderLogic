#if FALSE
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Decorator knowledge base class that, when answering queries, will concurrently execute the query and the negation of the query
    /// at the same time. This allows for returning a negative result in a reasonable timeframe when a query is known to be false, though
    /// at the cost of doing twice the work. And of course for statements that are unprovable either way, you may be waiting for a result
    /// for a long time.
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
            private DuplexResults? duplexResult;

            private DuplexKnowledgeBaseQuery(IQuery positiveQuery, IQuery negativeQuery)
            {
                PositiveQuery = positiveQuery;
                NegativeQuery = negativeQuery;
            }

            /// <inheritdoc/>
            public bool IsComplete => duplexResult.HasValue;

            /// <inheritdoc/>
            public bool Result
            {
                get
                {
                    if (!duplexResult.HasValue)
                    {
                        throw new InvalidOperationException("Query is not yet complete");
                    }

                    return duplexResult == DuplexResults.ProvenTrue;
                }
            }

            public DuplexResults DuplexResult
            {
                get
                {
                    if (!duplexResult.HasValue)
                    {
                        throw new InvalidOperationException("Query is not yet complete");
                    }

                    return duplexResult.Value;
                }
            }

            /// <summary>
            /// Gets the positive version of the query being carried out.
            /// </summary>
            public IQuery PositiveQuery { get; }

            /// <summary>
            /// Gets the negative version of the query being carried out.
            /// </summary>
            public IQuery NegativeQuery { get; }

            /// <inheritdoc/>
            public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
            {
                var resultIsKnownCts = new CancellationTokenSource();
                var effectiveCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, resultIsKnownCts.Token);
                var positiveQueryTask = PositiveQuery.ExecuteAsync(effectiveCts.Token);
                var negativeQueryTask = NegativeQuery.ExecuteAsync(effectiveCts.Token);

                // this could probably be written more succinctly..
                var completedQueryTask = await Task.WhenAny(positiveQueryTask, negativeQueryTask);
                if (completedQueryTask == positiveQueryTask)
                {
                    if (positiveQueryTask.Result == true)
                    {
                        duplexResult = DuplexResults.ProvenTrue;
                        resultIsKnownCts.Cancel();
                    }
                    else
                    {
                        var negativeQueryResult = await negativeQueryTask;
                        if (negativeQueryResult == true)
                        {
                            duplexResult = DuplexResults.ProvenFalse;
                        }
                        else
                        {
                            duplexResult = DuplexResults.Unproven;
                        }
                    }
                }
                else
                {
                    if (positiveQueryTask.Result == true)
                    {
                        duplexResult = DuplexResults.ProvenFalse;
                        resultIsKnownCts.Cancel();
                    }
                    else
                    {
                        var positiveQueryResult = await positiveQueryTask;
                        if (positiveQueryResult == true)
                        {
                            duplexResult = DuplexResults.ProvenTrue;
                        }
                        else
                        {
                            duplexResult = DuplexResults.Unproven;
                        }
                    }
                }

                await Task.WhenAll(positiveQueryTask, negativeQueryTask);
                return Result;
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

        public enum DuplexResults
        {
            ProvenTrue,
            ProvenFalse,
            Unproven,
        }
    }
}
#endif