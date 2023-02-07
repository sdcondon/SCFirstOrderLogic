using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
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
            // TODO: Yes, this is a slow implementation. It is simple, though - and thus will serve
            // well as a baseline for improvements. (I'm thinking including LiteralUnifier tweak so that it accepts multiple
            // literals and examines the tree for them all "simultaneously" - i.e. do full resolution, not binary).
            foreach (var clause1Literal in clause1.Literals)
            {
                foreach (var clause2Literal in clause2.Literals)
                {
                    if (LiteralUnifier.TryCreate(clause1Literal, clause2Literal.Negate(), out var resolvingUnifier))
                    {
                        var clause1OtherLiterals = clause1.Literals.Where(l => l != clause1Literal);
                        var clause2OtherLiterals = clause2.Literals.Where(l => l != clause2Literal);
                        HashSet<Literal> unifiedLiterals = new(clause1OtherLiterals.Concat(clause2OtherLiterals).Select(l => resolvingUnifier.ApplyTo(l)));

                        var factoringCarriedOut = false;
                        var clauseIsTriviallyTrue = false;
                        do
                        {
                            factoringCarriedOut = false;
                            var literalIndex = 0;
                            foreach (var literal in unifiedLiterals)
                            {
                                foreach (var otherLiteral in unifiedLiterals.Take(literalIndex))
                                {
                                    if (LiteralUnifier.TryCreate(literal, otherLiteral, out var factoringUnifier))
                                    {
                                        unifiedLiterals = new HashSet<Literal>(unifiedLiterals.Select(l => factoringUnifier.ApplyTo(l)));
                                        factoringCarriedOut = true;
                                        break;
                                    }

                                    if (literal.Equals(otherLiteral.Negate()))
                                    {
                                        clauseIsTriviallyTrue = true;
                                        break;
                                    }
                                }

                                if (factoringCarriedOut || clauseIsTriviallyTrue)
                                {
                                    break;
                                }

                                literalIndex++;
                            }
                        }
                        while (factoringCarriedOut && !clauseIsTriviallyTrue);

                        if (!clauseIsTriviallyTrue)
                        {
                            yield return new ClauseResolution(clause1, clause2, resolvingUnifier, new CNFClause(unifiedLiterals));
                        }
                    }
                }
            }
        }
    }
}
