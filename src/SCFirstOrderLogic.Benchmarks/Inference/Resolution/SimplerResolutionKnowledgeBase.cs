using SCFirstOrderLogic.Inference.Resolution.Utility;
using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// Even simpler than <see cref="SimpleResolutionKnowledgeBase"/> in that it doesn't use a clause store.
    /// </summary>
    public sealed class SimplerResolutionKnowledgeBase
    {
        private readonly List<CNFSentence> sentences = new(); // To be replaced with clause store
        private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
        private readonly Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimpleResolutionKnowledgeBase"/> class.
        /// </summary>
        /// <param name="clausePairFilter">A delegate to use to filter the pairs of clauses to be queued for a unification attempt. A true value indicates that the pair should be enqueued.</param>
        /// <param name="clausePairPriorityComparison">A delegate to use to compare the pairs of clauses to be queued for a unification attempt.</param>
        public SimplerResolutionKnowledgeBase(Func<(CNFClause, CNFClause), bool> clausePairFilter, Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison)
        {
            // NB: Throwing away clauses returned by the unifier store has performance impact. Could instead/also use a store that knows to not look for certain clause pairings in the first place..
            // However, REQUIRING the store to do this felt a little ugly from a code perspective, since the store is then a mix of implementation (how unifiers are stored/indexed) and strategy,
            // plus there's a bit more strategy in the form of the priority comparer. This feels a good compromise - there are of course alternatives (e.g. some kind of strategy object that encapsulates
            // both) - but they felt like overkill for this simple implementation.
            this.clausePairFilter = clausePairFilter; 
            this.clausePairPriorityComparison = clausePairPriorityComparison;
        }

        /// <inheritdoc />
        public void Tell(Sentence sentence) => sentences.Add(new CNFSentence(sentence));

        /// <inheritdoc />
        public bool Ask(Sentence sentence) => new Query(this, sentence).Complete();

        /// <summary>
        /// Creates an <see cref="IResolutionQuery"/> instance for fine-grained execution and examination of a query.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <returns>An <see cref="IResolutionQuery"/> instance that can be used to execute and examine the query.</returns>
        public Query CreateQuery(Sentence sentence) => new(this, sentence);

        public class Query
        {
            private readonly HashSet<CNFClause> clauses; // To be replaced with unifier store (scope thereof).
            private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
            private readonly MaxPriorityQueue<(CNFClause, CNFClause)> queue;
            private readonly Dictionary<CNFClause, (CNFClause, CNFClause, VariableSubstitution)> steps = new();

            private bool result;

            public Query(SimplerResolutionKnowledgeBase knowledgeBase, Sentence sentence)
            {
                this.NegatedQuery = new CNFSentence(new Negation(sentence));

                this.clauses = knowledgeBase.sentences
                    .Append(NegatedQuery)
                    .SelectMany(s => s.Clauses)
                    .ToHashSet();

                clausePairFilter = knowledgeBase.clausePairFilter;
                queue = new MaxPriorityQueue<(CNFClause, CNFClause)>(knowledgeBase.clausePairPriorityComparison);
                foreach (var ci in clauses)
                {
                    foreach (var cj in clauses)
                    {
                        if (knowledgeBase.clausePairFilter((ci, cj)))
                        {
                            queue.Enqueue((ci, cj));
                        }
                    }
                }
            }

            public CNFSentence NegatedQuery { get; }

            /// <inheritdoc/>
            public bool IsComplete { get; private set; } = false;

            /// <inheritdoc/>
            public bool Result
            {
                get
                {
                    if (!IsComplete)
                    {
                        throw new InvalidOperationException("Query is not yet complete");
                    }

                    return result;
                }
            }

            /// <inheritdoc/>
            public IReadOnlyDictionary<CNFClause, (CNFClause, CNFClause, VariableSubstitution)> Steps => steps;

            /// <inheritdoc/>
            public void NextStep()
            {
                if (IsComplete)
                {
                    throw new InvalidOperationException("Query is complete");
                }

                // Grab the next clause pairing from the queue..
                var (ci, cj) = queue.Dequeue();

                // ..and iterate through its resolvents (if any) - also make a note of the unifier so that we can include it in the record of steps that we maintain:
                foreach (var (unifier, resolvent) in ClauseResolver.Resolve(ci, cj))
                {
                    // If the resolvent is an empty clause, we've found a contradiction and can thus return a positive result:
                    if (resolvent.Equals(CNFClause.Empty))
                    {
                        steps[CNFClause.Empty] = (ci, cj, unifier);
                        result = true;
                        IsComplete = true;
                        return;
                    }

                    // Otherwise, check if we've found a new clause (i.e. something that we didn't know already)..
                    // NB: a limitation of this implementation - we only check if the clause is already present exactly -  we don't check for clauses that subsume it.
                    if (!clauses.Contains(resolvent))
                    {
                        // If this is a new clause, we queue up some more clause pairings - combinations of the resolvent and existing known clauses - adhering to any filtering and ordering we have in place.
                        foreach (var clause in clauses) // use unifier store lookup instead of all clauses
                        {
                            if (clausePairFilter((clause, resolvent)))
                            {
                                queue.Enqueue((clause, resolvent));
                            }
                        }

                        steps[resolvent] = (ci, cj, unifier);
                        clauses.Add(resolvent);
                    }
                }

                // If we've run out of clauses to smash together, return a negative result.
                if (queue.Count == 0)
                {
                    result = false;
                    IsComplete = true;
                }
            }

            /// <inheritdoc/>
            public bool Complete()
            {
                while (!IsComplete)
                {
                    NextStep();
                }

                return result;
            }
        }
    }
}
