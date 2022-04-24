using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Helpful extension methods for <see cref="IKnowledgeBase"/> instances.
    /// </summary>
    public static class IKnowledgeBaseExtensions
    {
        /// <summary>
        /// Inform a knowledge base that a given enumerable of sentences can all be assumed to hold true when answering queries.
        /// </summary>
        /// <param name="sentences">The sentences that can be assumed to hold true when ansering queries.</param>
        public static async Task TellAsync(this IKnowledgeBase knowledgeBase, IEnumerable<Sentence> sentences, CancellationToken cancellationToken = default)
        {
            foreach (var sentence in sentences)
            {
                await knowledgeBase.TellAsync(sentence, cancellationToken); // No guarentee of thread safety - go one at a time..
            }
        }
    }
}
