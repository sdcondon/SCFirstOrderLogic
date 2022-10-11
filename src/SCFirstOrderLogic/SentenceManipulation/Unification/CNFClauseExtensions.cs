using SCFirstOrderLogic;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.Unification
{
    /// <summary>
    /// Extension methods pertaining to the unification of clauses.
    /// <para/>
    /// NB: Of course, these methods COULD just be added to <see cref="CNFClause"/>. It felt a little messy
    /// to have CNFClause depend on code in the Unification namespace though. Perhaps a slightly idiosyncratic 
    /// design decision (after all, I'm fine with CNFClause depending on SentenceManipulation(, but I'm sticking by it
    /// </summary>
    public static class CNFClauseExtensions
    {
        /// <summary>
        /// Checks whether "this" clause unifies with any of an enumeration of other definite clauses.
        /// <para/>
        /// NB: this logic is not specific to definite clauses - so perhaps belongs elsewhere?
        /// </summary>
        /// <param name="thisClause">"This" clause.</param>
        /// <param name="clauses">The clauses to check for unification with.</param>
        /// <returns>True if this clause unifies with any of the provided clauses; otherwise false.</returns>
        public static bool UnifiesWithAnyOf(this CNFClause thisClause, IEnumerable<CNFClause> clauses)
        {
            return clauses.Any(c => thisClause.TryUnifyWith(c));
        }

        /// <summary>
        /// Tries to unify "this" clause with another.
        /// </summary>
        /// <param name="thisClause">"This" clause.</param>
        /// <param name="otherClause">The other clause.</param>
        /// <returns>True if the two clauses were successfully unified, otherwise false.</returns>
        private static bool TryUnifyWith(this CNFClause thisClause, CNFClause otherClause)
        {
            if (thisClause.Literals.Count != otherClause.Literals.Count)
            {
                return false;
            }
            
            return TryUnifyWith(thisClause.Literals, otherClause.Literals, new VariableSubstitution()).Any();
        }

        private static IEnumerable<VariableSubstitution> TryUnifyWith(IEnumerable<Literal> thisLiterals, IEnumerable<Literal> otherLiterals, VariableSubstitution unifier)
        {
            if (!thisLiterals.Any())
            {
                yield return unifier;
            }
            else
            {
                foreach (var otherLiteral in otherLiterals)
                {
                    var firstLiteralUnifier = new VariableSubstitution(unifier);

                    if (LiteralUnifier.TryUpdateUnsafe(thisLiterals.First(), otherLiteral, firstLiteralUnifier))
                    {
                        // Ugh: Skip is bad enough - Except is going to get slow, esp when nested. Important thing for now is that it works as a baseline..
                        foreach (var restOfLiteralsUnifier in TryUnifyWith(thisLiterals.Skip(1), otherLiterals.Except(new[] { otherLiteral }), firstLiteralUnifier))
                        {
                            yield return restOfLiteralsUnifier;
                        }
                    }
                }
            }
        }
    }
}
