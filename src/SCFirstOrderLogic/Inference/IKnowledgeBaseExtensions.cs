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
        /// <param name="knowledgeBase">The knowledge base to tell.</param>
        /// <param name="sentences">The sentences that can be assumed to hold true when answering queries.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        public static async Task TellAsync(this IKnowledgeBase knowledgeBase, IEnumerable<Sentence> sentences, CancellationToken cancellationToken = default)
        {
            foreach (var sentence in sentences)
            {
                // No guarantee of re-entrance safety - so go one at a time. TODO: this is dumb - not this class' responsibility
                await knowledgeBase.TellAsync(sentence, cancellationToken);
            }
        }

        /// <summary>
        /// Inform a knowledge base that a given enumerable of sentences can all be assumed to hold true when answering queries.
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base to tell.</param>
        /// <param name="sentences">The sentences that can be assumed to hold true when answering queries.</param>
        public static void Tell(this IKnowledgeBase knowledgeBase, IEnumerable<Sentence> sentences)
        {
            knowledgeBase.TellAsync(sentences).Wait();
        }

        /// <summary>
        /// Inform a knowledge base that a given sentence can all be assumed to hold true when answering queries.
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base to tell.</param>
        /// <param name="sentence">The sentence that can be assumed to hold true when answering queries.</param>
        public static void Tell(this IKnowledgeBase knowledgeBase, Sentence sentence)
        {
            knowledgeBase.TellAsync(sentence).Wait();
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// <para/>
        /// NB: We don't define an Ask(Async) method directly on the knowledge base interface because of this library's
        /// focus on learning outcomes. The <see cref="IQuery"/> interface allows for implementations that support step-by-step
        /// execution and/or examination of the steps that lead to the result. Note that "Ask" extension methods do exist,
        /// though (which create queries and immediately execute them to completion).
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base to create the query of.</param>
        /// <param name="sentence">The query sentence.</param>
        /// <returns>An <see cref="IQuery"/> implementation representing the query, that can then be used </returns>
        public static IQuery CreateQuery(this IKnowledgeBase knowledgeBase, Sentence sentence)
        {
            return knowledgeBase.CreateQueryAsync(sentence).Result;
        }

        /// <summary>
        /// Asks the knowledge base if a given sentence necessarily holds true, given what it knows.
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base to ask.</param>
        /// <param name="sentence">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>True if the sentence is known to be true, false if it is known to be false or cannot be determined.</returns>
        public static async Task<bool> AskAsync(this IKnowledgeBase knowledgeBase, Sentence sentence, CancellationToken cancellationToken = default)
        {
            using var query = await knowledgeBase.CreateQueryAsync(sentence, cancellationToken);
            return await query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// Asks the knowledge base if a given sentence necessarily holds true, given what it knows.
        /// </summary>
        /// <param name="knowledgeBase">The knowledge base to ask.</param>
        /// <param name="sentence">The query sentence.</param>
        public static bool Ask(this IKnowledgeBase knowledgeBase, Sentence sentence)
        {
            using var query = knowledgeBase.CreateQueryAsync(sentence).Result;
            return query.ExecuteAsync().Result;
        }
    }
}
