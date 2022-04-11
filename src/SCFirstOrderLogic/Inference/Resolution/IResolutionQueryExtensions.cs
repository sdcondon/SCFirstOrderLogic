using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Helpful extension methods for <see cref="IResolutionQuery"/> instances.
    /// </summary>
    public static class IResolutionQueryExtensions
    {
        /// <summary>
        /// Produces a (very raw) explanation of the steps that led to the result of the query.
        /// </summary>
        /// <returns>A (very raw) explanation of the steps that led to the result of the query.</returns>
        public static string Explain(this IResolutionQuery query)
        {
            if (!query.IsComplete)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }
            else if (!query.Result)
            {
                throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
            }

            // Walk back through the tree of steps (CNFClause.Empty will be the root), breadth-first:
            var orderedSteps = new List<CNFClause>();
            var queue = new Queue<CNFClause>(new[] { CNFClause.Empty });
            while (queue.Count > 0)
            {
                var clause = queue.Dequeue();
                if (orderedSteps.Contains(clause))
                {
                    // We have found the same clause "earlier" than another encounter of it.
                    // Remove the "later" one so that once we reverse the list, there are no
                    // references to clauses we've not seen yet.
                    orderedSteps.Remove(clause);
                }
                
                if (query.Steps.TryGetValue(clause, out var input))
                {
                    queue.Enqueue(input.Item1);
                    queue.Enqueue(input.Item2);

                    // NB: only record it as a step if its not a clause from the KB (or negated query) - explanation is clearer that way..
                    orderedSteps.Add(clause);
                }
            }

            orderedSteps.Reverse();
            var explanation = new StringBuilder();
            for (var i = 0; i < orderedSteps.Count; i++)
            {
                var input = query.Steps[orderedSteps[i]];

                string GetSource(CNFClause clause)
                {
                    if (orderedSteps.Contains(clause))
                    {
                        return $"#{orderedSteps.IndexOf(clause):D2}";
                    }
                    else if (query.NegatedQuery.Clauses.Contains(clause))
                    {
                        return " ¬Q";
                    }
                    else
                    {
                        return " KB";
                    }
                }

                explanation.AppendLine($"#{i:D2}: {orderedSteps[i]}");
                explanation.AppendLine($"     From {GetSource(input.Item1)}: {input.Item1}");
                explanation.AppendLine($"     And  {GetSource(input.Item2)}: {input.Item2} ");
                explanation.Append("     Using   : {");
                explanation.Append(string.Join(", ", input.Item3.Select(s => $"{s.Key}/{s.Value}")));
                explanation.AppendLine("}");

                explanation.AppendLine();
            }

            return explanation.ToString();
        }
    }
}
