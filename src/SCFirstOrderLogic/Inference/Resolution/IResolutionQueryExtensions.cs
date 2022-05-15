using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Retrieves a list of the (useful) clauses discovered by a query that has returned a positive result.
        /// Starts with clauses that were discovered using only clauses from the knowledge base or negated
        /// query, and ends with the empty clause.
        /// </summary>
        /// <param name="query">The query to retrieve the discovered clauses of.</param>
        /// <returns>A list of the discovered clauses of the given query.</returns>
        /// <exception cref="InvalidOperationException">If the query is not complete, or returned a negative result.</exception>
        public static ReadOnlyCollection<CNFClause> GetDiscoveredClauses(this IResolutionQuery query)
        {
            if (!query.IsComplete)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }
            else if (!query.Result)
            {
                throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
            }

            // Walk back through the DAG of clauses, starting from CNFClause.Empty, breadth-first:
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

                // If the clause is an intermediate one from the query (as opposed to one found
                // in the knowledge base or negated query)..
                if (query.Steps.TryGetValue(clause, out var input))
                {
                    // ..queue up the two clauses that resolved to give us this one..
                    queue.Enqueue(input.clause1);
                    queue.Enqueue(input.clause2);

                    // ..and record it in the steps list.
                    orderedSteps.Add(clause);
                }
            }

            // Reverse the steps encountered in the backward pass, to obtain a list of steps
            // contributing to the result, in (roughly..) the order in which query found them.
            orderedSteps.Reverse();

            return orderedSteps.AsReadOnly();
        }

        /// <summary>
        /// Returns an enumeration of all of the Terms created by the normalisation process (as opposed to featuring in the original sentences).
        /// That is, standardised variables and Skolem functions. Intended to be useful in creating a "legend" of such terms.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Term> FindNormalisationTerms(params CNFClause[] clauses)
        {
            var returnedAlready = new List<Term>();

            foreach (var literal in clauses.SelectMany(c => c.Literals))
            {
                foreach (var topLevelTerm in literal.Predicate.Arguments)
                {
                    var stack = new Stack<Term>();
                    stack.Push(topLevelTerm);
                    while (stack.Count > 0)
                    {
                        var term = stack.Pop();
                        switch (term)
                        {
                            case Function function:
                                if (function.Symbol is SkolemFunctionSymbol && !returnedAlready.Contains(function))
                                {
                                    returnedAlready.Add(function);
                                    yield return function;
                                }

                                foreach (var argument in function.Arguments)
                                {
                                    stack.Push(argument);
                                }

                                break;
                            case VariableReference variable:
                                if (variable.Symbol is StandardisedVariableSymbol && !returnedAlready.Contains(variable))
                                {
                                    returnedAlready.Add(variable);
                                    yield return variable;
                                }

                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Produces a (very raw) explanation of the steps that led to the result of the query.
        /// </summary>
        /// <returns>A (very raw) explanation of the steps that led to the result of the query.</returns>
        public static string Explain(this IResolutionQuery query)
        {
            var formatter = new SentenceFormatter();

            if (!query.IsComplete)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }
            else if (!query.Result)
            {
                throw new InvalidOperationException("Explanation of a negative result (which could be massive) is not supported");
            }

            var discoveredClauses = query.GetDiscoveredClauses();

            // Now build the explanation string.
            var explanation = new StringBuilder();
            for (var i = 0; i < discoveredClauses.Count; i++)
            {
                var (clause1, clause2, unifier) = query.Steps[discoveredClauses[i]];

                string GetSource(CNFClause clause)
                {
                    if (discoveredClauses.Contains(clause))
                    {
                        return $"#{discoveredClauses.IndexOf(clause):D2}";
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

                string ExplainNormalisationTerm(Term term)
                {
                    if (term is Function function && function.Symbol is SkolemFunctionSymbol skolemFunctionSymbol)
                    {
                        return $"some {formatter.GetOrCreateStandardisedVariableLabel(skolemFunctionSymbol.StandardisedVariableSymbol)} from {formatter.Print(skolemFunctionSymbol.OriginalSentence)}";
                    }
                    else if (term is VariableReference variable && variable.Symbol is StandardisedVariableSymbol standardisedVariableSymbol)
                    {
                        return $"a standardisation of {standardisedVariableSymbol.OriginalSymbol} from {formatter.Print(standardisedVariableSymbol.OriginalSentence)}";
                    }
                    else
                    {
                        return $"something unrecognised. Something has gone wrong..";
                    } 
                }

                explanation.AppendLine($"#{i:D2}: {formatter.Print(discoveredClauses[i])}");
                explanation.AppendLine($"     From {GetSource(clause1)}: {formatter.Print(clause1)}");
                explanation.AppendLine($"     And  {GetSource(clause2)}: {formatter.Print(clause2)} ");
                explanation.Append("     Using   : {");
                explanation.Append(string.Join(", ", unifier.Bindings.Select(s => $"{formatter.Print(s.Key)}/{formatter.Print(s.Value)}")));
                explanation.AppendLine("}");
                foreach (var term in FindNormalisationTerms(discoveredClauses[i], clause1, clause2))
                {
                    explanation.AppendLine($"     ..where {formatter.Print(term)} is {ExplainNormalisationTerm(term)}");
                }

                explanation.AppendLine();
            }

            return explanation.ToString();
        }
    }
}
