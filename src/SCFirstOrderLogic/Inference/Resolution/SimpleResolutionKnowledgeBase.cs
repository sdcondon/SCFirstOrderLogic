using SCFirstOrderLogic.Inference.Resolution.Utility;
using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// See §9.5 of Artificial Intelligence: A Modern Approach, for a detailed explanation of resolution.
    /// Notes:
    /// <list type="bullet">
    /// <item/>Includes functionality for fine-grained execution and examination of individual steps of queries.
    /// <item/>Has no in-built handling of equality (so, if equality appears in the knowledge base, its properties need to be axiomised - see §9.5.5 of Artifical Intelligence: A Modern Approach).
    /// <item/>Not thread-safe (i.e. not re-entrant) - despite the fact that resolution is ripe for parallelisation.
    /// </list>
    /// </summary>
    public sealed class SimpleResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly List<CNFSentence> sentences = new List<CNFSentence>(); // To be replaced with unifier store
        private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
        private readonly IComparer<(CNFClause, CNFClause)> clausePairPriorityComparer;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimpleResolutionKnowledgeBase"/> class.
        /// </summary>
        /// <param name="unifierStore"></param>
        /// <param name="clausePairFilter"></param>
        /// <param name="clausePairPriorityComparer"></param>
        public SimpleResolutionKnowledgeBase(/*IUnifierStore unifierStore, */Func<(CNFClause, CNFClause), bool> clausePairFilter, IComparer<(CNFClause, CNFClause)> clausePairPriorityComparer)
        {
            //this.unifierStore = unifierStore;
            // NB: throwing away clauses returned by the unifier store has performance impact. Could also use a store that knows to not look for certain clause pairings in the first place..
            // However, REQUIRING the store to do this felt a little ugly from a code perspective, since the store is then a mix of implementation (how unifiers are stored/indexed) and strategy,
            // plus there's a bit more strategy in the priority comparer.. This feels a good compromise - there are of course alternatives (e.g. some kind of strategy object that encapsulates
            // both) - but they felt like overkill for this Simple implementation.
            this.clausePairFilter = clausePairFilter; 
            this.clausePairPriorityComparer = clausePairPriorityComparer;
        }

        /// <inheritdoc />
        public void Tell(Sentence sentence) => sentences.Add(new CNFSentence(sentence));

        /// <inheritdoc />
        public bool Ask(Sentence sentence) => new Query(this, sentence).Complete();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IResolutionQuery CreateQuery(Sentence query) => new Query(this, query);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool Ask(Sentence sentence, out IResolutionQuery query)
        {
            query = new Query(this, sentence);
            return query.Complete();
        }

        /// <summary>
        /// book 9.5.2
        /// </summary>
        private class Query : IResolutionQuery
        {
            private readonly HashSet<CNFClause> clauses; // To be replaced with unifier store
            private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
            private readonly MaxPriorityQueue<(CNFClause, CNFClause)> queue;
            private readonly Dictionary<CNFClause, (CNFClause, CNFClause, IReadOnlyDictionary<VariableReference, Term>)> steps = new Dictionary<CNFClause, (CNFClause, CNFClause, IReadOnlyDictionary<VariableReference, Term>)>();

            private bool result;

            /// <summary>
            /// Initializes a new instance of the <see cref="SimplestResolutionQuery"/> class.
            /// </summary>
            /// <param name="knowledgeBase"></param>
            /// <param name="sentence"></param>
            public Query(SimpleResolutionKnowledgeBase knowledgeBase, Sentence sentence)
            {
                this.clauses = knowledgeBase.sentences
                    .Append(new CNFSentence(new Negation(sentence)))
                    .SelectMany(s => s.Clauses)
                    .ToHashSet();

                clausePairFilter = knowledgeBase.clausePairFilter;
                queue = new MaxPriorityQueue<(CNFClause, CNFClause)>(knowledgeBase.clausePairPriorityComparer);
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
            public IReadOnlyDictionary<CNFClause, (CNFClause, CNFClause, IReadOnlyDictionary<VariableReference, Term>)> Steps => steps;

            /// <inheritdoc/>
            public void NextStep()
            {
                if (IsComplete)
                {
                    throw new InvalidOperationException("Query is complete");
                }

                var (ci, cj) = queue.Dequeue();
                var resolvents = ClauseUnifier.Unify(ci, cj);

                foreach (var (unifier, resolvent) in resolvents)
                {
                    // If the resolvent is an empty clause, we've found a contradiction and can thus return a positive result:
                    if (resolvent.Equals(CNFClause.Empty))
                    {
                        steps[CNFClause.Empty] = (ci, cj, unifier.Substitutions);
                        result = true;
                        IsComplete = true;
                        return;
                    }

                    // Otherwise, queue up a bunch more clause pairs, adhering to any filtering and ordering we have in place.
                    // NB: We only check if the clause is already present exactly -  we don't check for clauses that subsume the resolvent.
                    if (!clauses.Contains(resolvent))
                    {
                        steps[resolvent] = (ci, cj, unifier.Substitutions);

                        foreach (var clause in clauses)
                        {
                            if (clausePairFilter((clause, resolvent)))
                            {
                                queue.Enqueue((clause, resolvent));
                            }
                        }

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
