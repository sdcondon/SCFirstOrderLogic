using System.Collections.Generic;

namespace SCFirstOrderLogic.KnowledgeBases
{
    /// <summary>
    /// Helpful extension methods for <see cref="IKnowledgeBase"/> instances.
    /// </summary>
    public static class IKnowledgeBaseExtensions
    {
        /// <summary>
        /// Inform a knowledge base that a given enumerable of sentences can all be assumed to hold true when answering queries.
        /// </summary>
        /// <param name="sentences">The sentences that are always true.</param>
        public static void Tell(this IKnowledgeBase knowledgeBase, IEnumerable<Sentence> sentences)
        {
            foreach (var sentence in sentences)
            {
                knowledgeBase.Tell(sentence);
            }
        }
    }
}
