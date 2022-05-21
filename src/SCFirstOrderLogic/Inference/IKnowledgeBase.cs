using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// A store of knowledge expressed as sentences of first-order logic.
    /// </summary>
    public interface IKnowledgeBase
    {
        /// <summary>
        /// Tells the knowledge base that a given sentence can be assumed to hold true when answering queries.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asks the knowledge base if a given sentence necessarily holds true, given what it knows.
        /// </summary>
        /// <param name="query">The sentence to ask about.</param>
        /// <returns>True if the sentence is known to be true, false if it is known to be false or cannot be determined.</returns>
        public Task<bool> AskAsync(Sentence query, CancellationToken cancellationToken = default);

        //// NB: No AskVars, yet anyway..
    }
}
