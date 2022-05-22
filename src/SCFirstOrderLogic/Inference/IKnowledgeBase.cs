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
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// <para/>
        /// NB: We don't define an AskAsync method directly on the knowledge base interface because of this library's
        /// focus on learning outcomes. Knowledge bases MUST use the <see cref="IQuery"/> interface to allow
        /// for fine-grained execution. Note that an <see cref="IKnowledgeBaseExtensions.AskAsync"/> extension method
        /// does exist, though (which creates a query and immediately executes it to completion).
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>An object representing the query, for fine-grained </returns>
        public Task<IQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default);

        //// NB: No AskVars, yet anyway..
    }
}
