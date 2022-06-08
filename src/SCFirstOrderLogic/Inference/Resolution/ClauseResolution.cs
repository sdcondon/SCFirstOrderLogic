using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Container for information about the (binary) resolution of some CNF clauses.
    /// </summary>
    public class ClauseResolution
    {
        /// <remarks>
        /// Private because it doesn't validate that the arguments actually represent a resolution.
        /// The intention is for instance creation to happen via the <see cref="Resolve"/> method.
        /// </remarks>
        private ClauseResolution(CNFClause clause1, CNFClause clause2, VariableSubstitution substitution, CNFClause resolvent)
        {
            Clause1 = clause1;
            Clause2 = clause2;
            Substitution = substitution;
            Resolvent = resolvent;
        }

        /// <summary>
        /// Gets the first of the two clauses that are resolved to give the resolvent clause (NB: for the moment at least, this class is specifically for binary resolution).
        /// </summary>
        public CNFClause Clause1 { get; }

        /// <summary>
        /// Gets the second of the two clauses that are resolved to give the resolvent clause (NB: for the moment at least, this class is specifically for binary resolution).
        /// </summary>
        public CNFClause Clause2 { get; }

        /// <summary>
        /// Gets the variable substitution that is applied to resolve the input clauses.
        /// </summary>
        public VariableSubstitution Substitution { get; }

        /// <summary>
        /// Gets the resolvent clause.
        /// </summary>
        public CNFClause Resolvent { get; }

        /// <summary>
        /// Attempts to resolve two clauses to potentially create some new clauses.
        /// </summary>
        /// <param name="clause1">The first of the clauses to resolve.</param>
        /// <param name="clause2">The second of the clauses to resolve.</param>
        /// <returns>Zero or more results, each consisting of a unifier and output clause.</returns>
        public static IEnumerable<ClauseResolution> Resolve(CNFClause clause1, CNFClause clause2)
        {
            // Yes, this is a slow implementation. It is simple, though - and thus will serve
            // well as a baseline for improvements. (I'm thinking including LiteralUnifier tweak so that it accepts multiple
            // literals and examines the tree for them all "simultaneously" - i.e. do full resolution, not binary).
            foreach (var literal1 in clause1.Literals)
            {
                foreach (var literal2 in clause2.Literals)
                {
                    if (LiteralUnifier.TryCreate(literal1, literal2.Negate(), out var unifier))
                    {
                        var unifiedLiterals = new HashSet<CNFLiteral>(clause1.Literals
                            .Concat(clause2.Literals)
                            .Except(new[] { literal1, literal2 })
                            .Select(l => unifier.ApplyTo(l)));

                        var factoringCarriedOut = false;
                        var clauseIsTriviallyTrue = false;
                        do
                        {
                            factoringCarriedOut = false;
                            foreach (var rLiteral1 in unifiedLiterals)
                            {
                                foreach (var rLiteral2 in unifiedLiterals)
                                {
                                    if (!rLiteral1.Equals(rLiteral2) && LiteralUnifier.TryCreate(rLiteral1, rLiteral2, out var factoringUnifier))
                                    {
                                        unifiedLiterals = new HashSet<CNFLiteral>(unifiedLiterals.Select(l => factoringUnifier.ApplyTo(l)));
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
                            yield return new ClauseResolution(clause1, clause2, unifier, new CNFClause(unifiedLiterals));
                        }
                    }
                }
            }
        }
    }
}
