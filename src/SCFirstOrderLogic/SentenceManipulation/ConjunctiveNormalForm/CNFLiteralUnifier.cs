using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Utility class for unifying literals.
    /// </summary>
    public class CNFLiteralUnifier
    {
        private CNFLiteralUnifier(Dictionary<VariableReference, Term> substitutions)
        {
            Substitutions = substitutions;
        }

        /// <summary>
        /// Gets the substitions made by this unifier.
        /// </summary>
        /// <remarks>
        /// TODO - Just returns a dictionary - should probably actually return an immutable type..
        /// </remarks>
        public IReadOnlyDictionary<VariableReference, Term> Substitutions { get; }

        /// <summary>
        /// Attempts to unify two literals.
        /// </summary>
        /// <param name="x">One of the two literals to attempt to unify.</param>
        /// <param name="y">One of the two literals to attempt to unify.</param>
        /// <param name="unifier">If the literals can be unified, this out parameter will be the unifier.</param>
        /// <returns>True if the two literals can be unified, otherwise false.</returns>
        public static bool TryCreate(CNFLiteral x, CNFLiteral y, [NotNullWhen(returnValue: true)] out CNFLiteralUnifier? unifier)
        {
            var substitions = new Dictionary<VariableReference, Term>();

            if (!TryUnify(x, y, substitions))
            {
                unifier = null;
                return false;
            }

            unifier = new CNFLiteralUnifier(substitions);
            return true;
        }

        public static bool TryUnify(CNFLiteral x, CNFLiteral y, [NotNullWhen(returnValue: true)] out CNFLiteral? unified)
        {
            if (!TryCreate(x, y, out var unifier))
            {
                unified = null;
                return false;
            }

            unified = unifier.ApplyTo(x);
            return true;
        }

        public CNFLiteral ApplyTo(CNFLiteral literal)
        {
            // should this complain if its not being applied to one of the literals it was created against?
            // or am I thinking about this wrong and we should always just be returning the unified literal?
            // wait and see..
            throw new NotImplementedException("Not yet implemented");
        }

        private static bool TryUnify(CNFLiteral x, CNFLiteral y, IDictionary<VariableReference, Term> unifier)
        {
            if (x.IsNegated != y.IsNegated || !x.Predicate.Symbol.Equals(y.Predicate.Symbol))
            {
                return false;
            }

            foreach (var args in x.Predicate.Arguments.Zip(y.Predicate.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryUnify(Term x, Term y, IDictionary<VariableReference, Term> unifier)
        {
            return (x, y) switch
            {
                (VariableReference variable, _) => TryUnify(variable, y, unifier),
                (_, VariableReference variable) => TryUnify(variable, x, unifier),
                (Function functionX, Function functionY) => TryUnify(functionX, functionY, unifier),
                _ => x.Equals(y), // only potential for equality is if they're both constants. Worth being explicit?
            };
        }

        private static bool TryUnify(VariableReference variable, Term other, IDictionary<VariableReference, Term> unifier)
        {
            if (unifier.TryGetValue(variable, out var value))
            {
                // The variable is already mapped to something - we need to make sure that the
                // mapping is consistent with the "other" value.
                return TryUnify(value, other, unifier);
            }
            else if (other is VariableReference otherVariable && unifier.TryGetValue(otherVariable, out value))
            {
                // The other value is also a variable that is already mapped to something - we need to make sure that the
                // mapping is consistent with the "other" value.
                return TryUnify(variable, value, unifier);
            }
            else if (Occurs(variable, other))
            {
                return false;
            }
            else
            {
                // This substitution is not in the book, but is so that e.g. Knows(John, X) and Knows(Y, Mother(Y)) will have x = Mother(John), not x = Mother(Y)
                other = new VariableSubstituter(unifier).ApplyTo(other); 
                unifier[variable] = other;
                return true;
            }
        }

        private static bool TryUnify(Function x, Function y, IDictionary<VariableReference, Term> unifier)
        {
            if (!x.Symbol.Equals(y.Symbol))
            {
                return false;
            }

            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Occurs(VariableReference variable, Term term)
        {
            // TODO*-PERFORMANCE: GC impact when creating a bunch of these.. Mutability and pooling?
            var finder = new VariableFinder(variable); 
            finder.ApplyTo(term);
            return finder.IsFound;
        }

        private class VariableFinder : SentenceTransformation
        {
            private readonly VariableReference variableReference;

            public VariableFinder(VariableReference variableReference) => this.variableReference = variableReference;

            public bool IsFound { get; private set; } = false;

            // TODO-PERFORMANCE: For performance, should probably override everything and stop as soon as IsFound is true.
            // And/or establish visitor pattern to make this easier..

            protected override Term ApplyTo(VariableReference variable)
            {
                if (variable.Equals(variableReference))
                {
                    IsFound = true;
                }

                return variable;
            }
        }

        private class VariableSubstituter : SentenceTransformation
        {
            private readonly IDictionary<VariableReference, Term> variableSubstitutions;

            public VariableSubstituter(IDictionary<VariableReference, Term> variableSubstitutions) => this.variableSubstitutions = variableSubstitutions;

            protected override Term ApplyTo(VariableReference variable)
            {
                if (variableSubstitutions.TryGetValue(variable, out var substitutedTerm))
                {
                    return substitutedTerm;
                }

                return variable;
            }
        }
    }
}
