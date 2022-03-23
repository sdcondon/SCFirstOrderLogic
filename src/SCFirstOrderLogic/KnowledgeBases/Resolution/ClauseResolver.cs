using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.KnowledgeBases.Resolution
{
    public static class ClauseResolver
    {
        /// <summary>
        /// Resolves two clauses to potentially create some new clauses.
        /// </summary>
        /// <param name="clause1">The first of the clauses to resolve.</param>
        /// <param name="clause2">The second of the clauses to resolve.</param>
        /// <returns>A new clause.</returns>
        public static IEnumerable<(CNFClause resolvent, CNFLiteralUnifier unifier)> Resolve(CNFClause clause1, CNFClause clause2)
        {
            // Yes, this is a slow implementation. It is simple, though - and thus will serve
            // well as a baseline for improvements. (I'm thinking of a CNFLiteralUnifier that accepts multiple
            // literals and examines the tree for them all "simultaneously" - i.e. do full resolution, not binary).
            foreach (var literal1 in clause1.Literals)
            {
                foreach (var literal2 in clause2.Literals)
                {
                    if (CNFLiteralUnifier.TryCreate(literal1, literal2.Negate(), out var unifier))
                    {
                        var resolventLiterals = new HashSet<CNFLiteral>(clause1.Literals
                            .Concat(clause2.Literals)
                            .Except(new[] { literal1, literal2 })
                            .Select(l => unifier.ApplyTo(l)));

                        var factoringCarriedOut = false;
                        var clauseIsTriviallyTrue = false;
                        do
                        {
                            factoringCarriedOut = false;
                            foreach (var rLiteral1 in resolventLiterals)
                            {
                                foreach (var rLiteral2 in resolventLiterals)
                                {
                                    if (!rLiteral1.Equals(rLiteral2) && CNFLiteralUnifier.TryCreate(rLiteral1, rLiteral2, out var factoringUnifier))
                                    {
                                        resolventLiterals = new HashSet<CNFLiteral>(resolventLiterals.Select(l => factoringUnifier.ApplyTo(l)));
                                        factoringCarriedOut = true;
                                        break;
                                    }

                                    if (rLiteral1.Predicate.Equals(rLiteral2) && rLiteral1.IsPositive != rLiteral2.IsPositive)
                                    {
                                        clauseIsTriviallyTrue = true;
                                        break;
                                    }
                                }

                                if (factoringCarriedOut || clauseIsTriviallyTrue)
                                {
                                    break;
                                }
                            }
                        }
                        while (factoringCarriedOut);

                        if (!clauseIsTriviallyTrue)
                        {
                            yield return (new CNFClause(resolventLiterals), unifier);
                        }
                    }
                }
            }
        }
    }
}
