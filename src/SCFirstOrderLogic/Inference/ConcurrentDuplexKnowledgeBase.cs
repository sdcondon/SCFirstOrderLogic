#if FALSE
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Decorator knowledge base class that, when answering queries, will concurrently execute the query and the negation of the query
    /// at the same time. This allows for returning a negative result in a reasonble timeframe when a query is known to be false.
    /// It does of course still return the same result for false and unknown - despite in theory being able to distinguish between the two scenarios
    /// <para/>
    /// No reference back to the source material for this one.
    /// </summary>
    public class ConcurrentDuplexKnowledgeBase : IKnowledgeBase
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
        public bool Ask(Sentence query)
        {
            var positiveQuery = Task.Run(() => innerKnowledgeBase.Ask(query));
            var negativeQuery = Task.Run(() => innerKnowledgeBase.Ask(new Negation(query)));
            return Task
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
                })
                .Result;
        }

        /// <inheritdoc/>
        public void Tell(Sentence sentence) => innerKnowledgeBase.Tell(sentence);
    }
}
#endif