using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// Includes functionality for fine-grained execution and examination of individual steps of queries.
    /// Has no in-built handling of equality (so, if equality appears in the knowledge base, requires its properties to be axiomised within the knowledge base - see §9.5.5 of Artifical Intelligence: A Modern Approach).
    /// </summary>
    public sealed class SimplestResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly List<CNFSentence> sentences = new List<CNFSentence>(); // To be replaced with unifier store

        /// <inheritdoc />
        public void Tell(Sentence sentence) => sentences.Add(new CNFSentence(sentence));

        /// <inheritdoc />
        public bool Ask(Sentence sentence) => new Query(this, sentence).Complete();

        /// <summary>
        /// Creates a new <see cref="IResolutionQuery"/>, for fine-grained execution and examination of a query.
        /// </summary>
        /// <param name="sentence">The sentence to query the truth of.</param>
        /// <returns>A new <see cref="IResolutionQuery"/> instance.</returns>
        public IResolutionQuery CreateQuery(Sentence sentence) => new Query(this, sentence);

        /// <summary>
        /// book 9.5.2
        /// TODO-BUG: Need to account for equality (assuming we don't want to axiomise..). could this be part of strategy..?
        /// </summary>
        private class Query : IResolutionQuery
        {
            private readonly HashSet<CNFClause> clauses; // To be replaced with unifier store
            private readonly Queue<(CNFClause, CNFClause)> queue = new Queue<(CNFClause, CNFClause)>(); // To be replaced with clause pair priority comparer & priority queue
            private readonly Dictionary<CNFClause, (CNFClause, CNFClause, IReadOnlyDictionary<VariableReference, Term>)> steps = new Dictionary<CNFClause, (CNFClause, CNFClause, IReadOnlyDictionary<VariableReference, Term>)>();

            private bool result;

            /// <summary>
            /// Initializes a new instance of the <see cref="SimplestResolutionQuery"/> class.
            /// </summary>
            /// <param name="knowledgeBase"></param>
            /// <param name="sentence"></param>
            public Query(SimplestResolutionKnowledgeBase knowledgeBase, Sentence sentence)
            {
                this.clauses = knowledgeBase.sentences
                    .Append(new CNFSentence(new Negation(sentence)))
                    .SelectMany(s => s.Clauses)
                    .ToHashSet();

                foreach (var ci in clauses)
                {
                    foreach (var cj in clauses)
                    {
                        queue.Enqueue((ci, cj));
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
                    if (resolvent.Equals(CNFClause.Empty))
                    {
                        steps[CNFClause.Empty] = (ci, cj, unifier.Substitutions);
                        result = true;
                        IsComplete = true;
                        return;
                    }

                    if (!clauses.Contains(resolvent))
                    {
                        steps[resolvent] = (ci, cj, unifier.Substitutions);

                        foreach (var clause in clauses)
                        {
                            queue.Enqueue((clause, resolvent));
                        }

                        clauses.Add(resolvent);
                    }
                }

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
