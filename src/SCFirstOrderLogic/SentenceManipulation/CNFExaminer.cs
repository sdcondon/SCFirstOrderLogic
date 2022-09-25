﻿using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// Utility logic for interrogations of sentences - notably, the normalisation terms (standardised variables and Skolem functions) within them.
    /// </summary>
    public static class CNFExaminer
    {
        /// <summary>
        /// Returns an enumerable of all of the Terms created by the normalisation process (as opposed to featuring in the original sentences).
        /// That is, standardised variables and Skolem functions. Intended to be useful in creating a "legend" of such terms.
        /// </summary>
        /// <returns>An enumerable of the (distinct) standardised variable references and Skolem functions found within the clauses.</returns>
        public static IEnumerable<Term> FindNormalisationTerms(params CNFClause[] clauses)
        {
            var predicates = clauses.SelectMany(c => c.Literals).Select(l => l.Predicate);
            return FindNormalisationTerms(predicates);
        }

        /// <summary>
        /// Returns an enumerable of all of the Terms created by the normalisation process (as opposed to featuring in the original sentences).
        /// That is, standardised variables and Skolem functions. Intended to be useful in creating a "legend" of such terms.
        /// </summary>
        /// <returns>An enumerable of the (distinct) standardised variable references and Skolem functions found within the clauses.</returns>
        public static IEnumerable<Term> FindNormalisationTerms(IEnumerable<Predicate> predicates)
        {
            var returnedAlready = new List<Term>();

            foreach (var predicate in predicates)
            {
                foreach (var topLevelTerm in predicate.Arguments)
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
    }
}