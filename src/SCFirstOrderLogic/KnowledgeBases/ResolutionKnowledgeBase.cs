using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.KnowledgeBases
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: doesn't yet allow any specification of resolution strategy. Essentially just works through pairs
    /// starting with the first sentences told to the knowledge base.
    /// </remarks>
    public class ResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly List<CNFSentence> sentences = new List<CNFSentence>();

        /// <inheritdoc />
        public void Tell(Sentence sentence)
        {
            // TODO-BUG: Need to standardise variables apart across all sentences..
            sentences.Add(new CNFSentence(sentence));
        }

        /// <inheritdoc />
        public bool Ask(Sentence sentence)
        {
            // TODO-BUG: Need to add factoring
            // TODO-BUG: Need to account for equality (assuming we don't want to axiomise..)

            var negationOfQueryAsCnf = new CNFSentence(new Negation(sentence));
            var clauses = sentences.Append(negationOfQueryAsCnf).SelectMany(s => s.Clauses).ToHashSet();
            var queue = new Queue<(CNFClause, CNFClause)>();

            foreach (var ci in clauses)
            {
                foreach (var cj in clauses)
                {
                    queue.Enqueue((ci, cj));
                }
            }

            while (queue.Count > 0)
            {
                var (ci, cj) = queue.Dequeue();
                var resolvents = CNFClause.Resolve(ci, cj);

                foreach (var resolvent in resolvents)
                {
                    if (resolvent.Equals(CNFClause.Empty))
                    {
                        return true;
                    }

                    if (!clauses.Contains(resolvent))
                    {
                        foreach (var clause in clauses)
                        {
                            queue.Enqueue((clause, resolvent));
                        }

                        clauses.Add(resolvent);
                    }
                }
            }

            return false;
        }
    }
}
