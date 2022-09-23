using SCFirstOrderLogic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.Unification
{
    /// <summary>
    /// Extension methods pertaining to the unification of clauses.
    /// <para/>
    /// NB: Of course, these methods COULD just be added to <see cref="CNFClause"/>. It felt a little messy
    /// to have CNFClause depend on code in the SentenceManipulation namespace though. Perhaps a slightly idiosyncratic 
    /// design decision, but I'm sticking by it..
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
            return clauses.Any(c => thisClause.TryUnifyWith(c, out var _));
        }

        /// <summary>
        /// Tries to unify "this" clause with another.
        /// </summary>
        /// <param name="thisClause">"This" clause.</param>
        /// <param name="otherClause">The other clause.</param>
        /// <param name="unifier">On success, will be populated with the unifier.</param>
        /// <returns>True if the two clauses were successfully unified, otherwise false.</returns>
        private static bool TryUnifyWith(this CNFClause thisClause, CNFClause otherClause, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
        {
            if (thisClause.Literals.Count != otherClause.Literals.Count)
            {
                unifier = null;
                return false;
            }

            unifier = new VariableSubstitution();

            foreach (var (literal1, literal2) in thisClause.Literals.Zip(otherClause.Literals))
            {
                if (!LiteralUnifier.TryUpdateUnsafe(literal1, literal2, unifier))
                {
                    unifier = null;
                    return false;
                }
            }

            return true;
        }
    }
}
