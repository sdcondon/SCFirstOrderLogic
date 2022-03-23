using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCFirstOrderLogic.KnowledgeBases.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// Includes functionality for fine-grained execution and examination of individual steps of queries.
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: doesn't yet allow any specification of resolution strategy. Essentially just works through pairs
    /// starting with the first sentences told to the knowledge base.
    /// </remarks>
    public class ResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly List<CNFSentence> sentences = new List<CNFSentence>(); // Ultimately to be replaced with unifier store

        /// <inheritdoc />
        public void Tell(Sentence sentence) => sentences.Add(new CNFSentence(sentence));

        /// <inheritdoc />
        public bool Ask(Sentence sentence) => CreateQuery(sentence).Complete();

        /// <summary>
        /// Creates a new <see cref="ResolutionQuery"/>, for fine-grained execution and examination of a query.
        /// </summary>
        /// <param name="sentence">The sentence to query the truth of.</param>
        /// <returns>A new <see cref="ResolutionQuery"/> instance.</returns>
        public ResolutionQuery CreateQuery(Sentence sentence) => new ResolutionQuery(this, sentence);

        /// <summary>
        /// book 9.5.2
        /// TODO-BUG: Need to account for equality (assuming we don't want to axiomise..). could this be part of strategy..?
        /// </summary>
        public class ResolutionQuery
        {
            private readonly HashSet<CNFClause> clauses; // Ultimately to be replaced with strategy & unifier store
            private readonly Queue<(CNFClause, CNFClause)> queue = new Queue<(CNFClause, CNFClause)>(); // Ultimately to be replaced with strategy & unifier store
            private readonly Dictionary<CNFClause, (CNFClause, CNFClause, CNFLiteralUnifier)> steps = new Dictionary<CNFClause, (CNFClause, CNFClause, CNFLiteralUnifier)>();

            private bool result;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResolutionQuery"/> class.
            /// </summary>
            /// <param name="knowledgeBase"></param>
            /// <param name="sentence"></param>
            public ResolutionQuery(ResolutionKnowledgeBase knowledgeBase, Sentence sentence)
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

            /// <summary>
            /// Gets a value indicating whether the query is complete.
            /// The result of completed queries is available via the <see cref="Result"/> property.
            /// Calling <see cref="NextStep"/> on a completed query will result in an <see cref="InvalidOperationException"/>.
            /// </summary>
            public bool IsComplete { get; private set; } = false;

            /// <summary>
            /// Gets a value indicating the result of the query.
            /// True indicates that the query is known to be true.
            /// False indicates that the query is known to be false or cannot be determined.
            /// </summary>
            /// <exception cref="InvalidOperationException">The query is not yet complete.</exception>
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

            /// <summary>
            /// Gets a mapping from a clause to the two clauses from which it was inferred.
            /// </summary>
            public IReadOnlyDictionary<CNFClause, (CNFClause, CNFClause, CNFLiteralUnifier)> Steps => steps;

            /// <summary>
            /// Executes the next step of the query.
            /// </summary>
            public void NextStep()
            {
                if (IsComplete)
                {
                    throw new InvalidOperationException("Query is complete");
                }

                var (ci, cj) = queue.Dequeue();
                var resolvents = ClauseResolver.Resolve(ci, cj);

                foreach (var (resolvent, unifier) in resolvents)
                {
                    if (resolvent.Equals(CNFClause.Empty))
                    {
                        steps[CNFClause.Empty] = (ci, cj, unifier);
                        result = true;
                        IsComplete = true;
                        return;
                    }

                    if (!clauses.Contains(resolvent))
                    {
                        steps[resolvent] = (ci, cj, unifier);

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

            /// <summary>
            /// Continuously 
            /// </summary>
            /// <returns></returns>
            public bool Complete()
            {
                while (!IsComplete)
                {
                    NextStep();
                }

                return result;
            }

            /// <summary>
            /// Produces a (quite raw) set of 
            /// </summary>
            /// <returns></returns>
            public string Explain()
            {
                if (!IsComplete)
                {
                    throw new InvalidOperationException("Query is not yet complete");
                }
                else if (!Result)
                {
                    throw new NotSupportedException("Explanation of a negative result is not yet supported");
                }

                // walk back through the tree of steps (CNFClause.Empty will be the root), breadth-first
                var orderedSteps = new List<CNFClause>();
                var queue = new Queue<CNFClause>(new[] { CNFClause.Empty });
                while (queue.Count > 0)
                {
                    var clause = queue.Dequeue();
                    orderedSteps.Add(clause);

                    if (steps.TryGetValue(clause, out var input))
                    {
                        queue.Enqueue(input.Item1);
                        queue.Enqueue(input.Item2);
                    }
                }

                orderedSteps.Reverse();
                var explanation = new StringBuilder();
                explanation.AppendLine($"Query is {Result}");
                for (var i = 0; i < orderedSteps.Count; i++)
                {
                    if (steps.TryGetValue(orderedSteps[i], out var input))
                    {
                        explanation.AppendLine($"#{i}: {orderedSteps[i]}");
                        explanation.AppendLine($"\tFrom #{orderedSteps.IndexOf(input.Item1)}: {input.Item1}");
                        explanation.AppendLine($"\tAnd  #{orderedSteps.IndexOf(input.Item2)}: {input.Item2} ");
                        explanation.Append("\tUsing  : {");
                        explanation.Append(string.Join(", ", input.Item3.Substitutions.Select(s => $"{s.Key}/{s.Value}")));
                        explanation.AppendLine("}");
                    }
                    else
                    {
                        explanation.AppendLine($"#{i}: {orderedSteps[i]}");
                        explanation.AppendLine($"\tFrom known facts");
                    }

                    explanation.AppendLine();
                }

                return explanation.ToString();
            }
        }
    }
}
