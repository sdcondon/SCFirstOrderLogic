#if FALSE
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
        /// Initialises a new instance of the <see cref="EqualityAxiomisingKnowledgeBase"/> class.
        /// </summary>
        public ConcurrentDuplexKnowledgeBase(IKnowledgeBase innerKnowledgeBase)
        {
            this.innerKnowledgeBase = innerKnowledgeBase;
        }

        /// <inheritdoc/>
        public async Task<bool> AskAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            var positiveQuery = innerKnowledgeBase.AskAsync(query, cancellationToken);
            var negativeQuery = innerKnowledgeBase.AskAsync(new Negation(query), cancellationToken));
            return await Task
                .WhenAny(positiveQuery, negativeQuery)
                .ContinueWith(t =>
                {
                    if (t.Result == positiveQuery)
                    {
                        return t.Result.Result;
                    }
                    else
                    {
                        return !t.Result.Result;
                    }
                }, cancellationToken);
        }

        /// <inheritdoc/>
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default) => innerKnowledgeBase.TellAsync(sentence, cancellationToken);
    }
}
#endif