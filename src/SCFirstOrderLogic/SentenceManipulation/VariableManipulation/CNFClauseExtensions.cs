// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation
{
    /// <summary>
    /// Useful variable inspection and manipulation methods for <see cref="CNFClause"/> instances.
    /// </summary>
    public static class CNFClauseExtensions
    {
        /// <summary>
        /// <para>
        /// Gets a value indicating whether 'this' clause subsumes another.
        /// </para>
        /// <para>
        /// That is, whether the other clause is logically entailed by this one.
        /// </para>
        /// </summary>
        /// <param name="thisClause"></param>
        /// <param name="otherClause"></param>
        /// <returns>True if this clause subsumes the other; otherwise false.</returns>
        public static bool Subsumes(this CNFClause thisClause, CNFClause otherClause)
        {
            if (thisClause.IsEmpty)
            {
                return false;
            }

            VariableSubstitution substitution = new();

            foreach (var literal in thisClause.Literals)
            {
                if (!otherClause.Literals.Any(l => InstanceUnifier.TryUpdate(literal, l, ref substitution)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether another clause subsumes this one.
        /// </para>
        /// <para>
        /// That is, whether this clause is logically entailed by the other.
        /// </para>
        /// </summary>
        /// <param name="thisClause"></param>
        /// <param name="otherClause"></param>
        /// <returns>True if the other clause subsumes this one; otherwise false.</returns>
        public static bool IsSubsumedBy(this CNFClause thisClause, CNFClause otherClause)
        {
            return otherClause.Subsumes(thisClause);
        }

        /// <summary>
        /// <para>
        /// Checks whether "this" clause unifies with any of an enumeration of other definite clauses.
        /// </para>
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
                    if (Unifier.TryUpdate(thisLiterals.First(), otherLiteral, unifier, out var firstLiteralUnifier))
                    {
                        // TODO-PERFORMANCE: Ugh, skip is bad enough - Except is going to get slow, esp when nested. Important thing for now is that it works as a baseline..
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
