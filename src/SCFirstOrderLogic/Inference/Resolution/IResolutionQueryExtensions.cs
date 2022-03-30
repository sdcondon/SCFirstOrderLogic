using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
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
                throw new NotSupportedException("Explanation of a negative result is not yet supported");
            }

            // walk back through the tree of steps (CNFClause.Empty will be the root), breadth-first
            var orderedSteps = new List<CNFClause>();
            var queue = new Queue<CNFClause>(new[] { CNFClause.Empty });
            while (queue.Count > 0)
            {
                var clause = queue.Dequeue();
                orderedSteps.Add(clause);

                if (query.Steps.TryGetValue(clause, out var input))
                {
                    queue.Enqueue(input.Item1);
                    queue.Enqueue(input.Item2);
                }
            }

            orderedSteps.Reverse();
            var explanation = new StringBuilder();
            explanation.AppendLine($"Query is {query.Result}");
            for (var i = 0; i < orderedSteps.Count; i++)
            {
                if (query.Steps.TryGetValue(orderedSteps[i], out var input))
                {
                    explanation.AppendLine($"#{i}: {orderedSteps[i]}");
                    explanation.AppendLine($"\tFrom #{orderedSteps.IndexOf(input.Item1)}: {input.Item1}");
                    explanation.AppendLine($"\tAnd  #{orderedSteps.IndexOf(input.Item2)}: {input.Item2} ");
                    explanation.Append("\tUsing  : {");
                    explanation.Append(string.Join(", ", input.Item3.Select(s => $"{s.Key}/{s.Value}")));
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
