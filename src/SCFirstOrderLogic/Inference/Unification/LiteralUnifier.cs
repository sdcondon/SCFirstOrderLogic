using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Utility class for creating unifiers for literals.
    /// See §9.2.2 ("Unification") of 'Artificial Intelligence: A Modern Approach' for an explanation of this algorithm.
    /// </summary>
    public static class LiteralUnifier
    {
        /// <summary>
        /// Attempts to create the most general unifier for two literals.
        /// </summary>
        /// <param name="x">One of the two literals to attempt to create a unifier for.</param>
        /// <param name="y">One of the two literals to attempt to create a unifier for.</param>
        /// <param name="unifier">If the literals can be unified, this out parameter will be the unifier.</param>
        /// <returns>True if the two literals can be unified, otherwise false.</returns>
        public static bool TryCreate(CNFLiteral x, CNFLiteral y, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
        {
            var unifierAttempt = new VariableSubstitution();

            if (!TryUpdate(x, y, unifierAttempt))
            {
                unifier = null;
                return false;
            }

            unifier = unifierAttempt;
            return true;
        }

        /// <summary>
        /// Attempts to update a unifier so that it (also) unifies two given literals.
        /// </summary>
        /// <param name="x">One of the two literals to attempt to create a unifier for.</param>
        /// <param name="y">One of the two literals to attempt to create a unifier for.</param>
        /// <param name="unifier">The unifier to update. NB: Can be partially updated on failure. This behaviour is to facilitate efficiency. Copy the unifier before invocation if you need to avoid this.</param>
        /// <returns>True if the two literals can be unified, otherwise false.</returns>
        public static bool TryUpdate(CNFLiteral x, CNFLiteral y, VariableSubstitution unifier)
        {
            if (x.IsNegated != y.IsNegated || !x.Predicate.Symbol.Equals(y.Predicate.Symbol))
            {
                return false;
            }

            // BUG: Makes the assumption that same symbol means same number of arguments.
            // It is possible to confuse this algorithm by passing literals where that isn't true
            foreach (var args in x.Predicate.Arguments.Zip(y.Predicate.Arguments, (x, y) => (x, y)))
            {
                if (!TryUpdate(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryUpdate(Term x, Term y, VariableSubstitution unifier)
        {
            return (x, y) switch
            {
                (VariableReference variable, _) => TryUpdate(variable, y, unifier),
                (_, VariableReference variable) => TryUpdate(variable, x, unifier),
                (Function functionX, Function functionY) => TryUpdate(functionX, functionY, unifier),
                // only potential for equality is if they're both constants. Perhaps worth testing this vs that explicitly and a default that just returns false.
                // Very similar from a performace perspective (constant equality does type check)
                _ => x.Equals(y),
            };
        }

        private static bool TryUpdate(VariableReference variable, Term other, VariableSubstitution unifier)
        {
            if (unifier.Bindings.TryGetValue(variable, out var value))
            {
                // The variable is already mapped to something - we need to make sure that the
                // mapping is consistent with the "other" value.
                return TryUpdate(value, other, unifier);
            }
            else if (other is VariableReference otherVariable && unifier.Bindings.TryGetValue(otherVariable, out value))
            {
                // The other value is also a variable that is already mapped to something - we need to make sure that the
                // mapping is consistent with the "other" value.
                return TryUpdate(variable, value, unifier);
            }
            else if (Occurs(variable, other))
            {
                return false;
            }
            else
            {
                // This substitution is not in the source book, but is so that e.g. unifying Knows(John, X) and Knows(Y, Mother(Y)) will give { X / Mother(John) }, not { X / Mother(Y) }
                // Might be duplicated effort in the broader scheme of things, but time will tell.
                other = unifier.ApplyTo(other);
                unifier.AddBinding(variable, other);
                return true;
            }
        }

        private static bool TryUpdate(Function x, Function y, VariableSubstitution unifier)
        {
            if (!x.Symbol.Equals(y.Symbol))
            {
                return false;
            }

            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
            {
                if (!TryUpdate(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Occurs(VariableReference variableReference, Term term)
        {
            return term switch
            {
                Constant c => false,
                VariableReference v => variableReference.Equals(v),
                Function f => f.Arguments.Any(a => Occurs(variableReference, a)),
                _ => throw new ArgumentException($"Unexpected term type '{term.GetType()}' encountered", nameof(term)),
            };
        }
    }
}
