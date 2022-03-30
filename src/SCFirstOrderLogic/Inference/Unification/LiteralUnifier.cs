using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Utility class for unifying literals.
    /// </summary>
    public class LiteralUnifier
    {
        private readonly VariableSubstitution variableSubstitution = new VariableSubstitution();

        /// <summary>
        /// Gets the substitions made by this unifier.
        /// </summary>
        /// <remarks>
        /// TODO: Just returns a dictionary (so could be interfered with by casting) - should probably actually return an immutable type. But not a big deal.
        /// </remarks>
        public IReadOnlyDictionary<VariableReference, Term> Substitutions => variableSubstitution.Bindings;

        /// <summary>
        /// Attempts to create the most general unifier for two literals.
        /// </summary>
        /// <param name="x">One of the two literals to attempt to create a unifier for.</param>
        /// <param name="y">One of the two literals to attempt to create a unifier for.</param>
        /// <param name="unifier">If the literals can be unified, this out parameter will be the unifier (which can then be applied with <see cref="ApplyTo"/>).</param>
        /// <returns>True if the two literals can be unified, otherwise false.</returns>
        public static bool TryCreate(CNFLiteral x, CNFLiteral y, [NotNullWhen(returnValue: true)] out LiteralUnifier? unifier)
        {
            var unifierAttempt = new LiteralUnifier();

            if (!TryUnify(x, y, unifierAttempt))
            {
                unifier = null;
                return false;
            }

            unifier = unifierAttempt;
            return true;
        }

        /// <summary>
        /// Attempts to unify two literals with their most general unifier.
        /// </summary>
        /// <param name="x">One of the two literals to attempt to unify.</param>
        /// <param name="y">One of the two literals to attempt to unify.</param>
        /// <param name="unified">If the literals can be unified, this out parameter will be the unified version of the literals.</param>
        /// <returns>True if the two literals can be unified, otherwise false.</returns>
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

        /// <summary>
        /// Applies the unifier to a literal.
        /// </summary>
        /// <param name="literal">The literal to apply the unifier to.</param>
        /// <returns>The unified version of the literal.</returns>
        public CNFLiteral ApplyTo(CNFLiteral literal)
        {
            // should this complain if its not being applied to one of the literals it was created against?
            // or am I thinking about this wrong and we should always just be returning the unified literal?
            // wait and see..

            // TODO-PERFORMANCE / TODO-MAINTAINABILITY: Also, think about not using SentenceTransformation here - perhaps create CNFLiteralTransformation
            // (or just making VariableSubstitution contain the logic itself - creating a base class when there's only one implementation is needless complexity)
            var literalAsSentence = variableSubstitution.ApplyTo(literal.IsNegated ? (Sentence)new Negation(literal.Predicate) : literal.Predicate);
            return new CNFLiteral(literalAsSentence);
        }

        private static bool TryUnify(CNFLiteral x, CNFLiteral y, LiteralUnifier unifier)
        {
            if (x.IsNegated != y.IsNegated || !x.Predicate.Symbol.Equals(y.Predicate.Symbol))
            {
                return false;
            }

            // BUG?: Makes the assumption that same symbol means same number of arguments.
            // It is possible to confuse this algorithm by passing literals where that isn't true
            foreach (var args in x.Predicate.Arguments.Zip(y.Predicate.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryUnify(Term x, Term y, LiteralUnifier unifier)
        {
            return (x, y) switch
            {
                (VariableReference variable, _) => TryUnify(variable, y, unifier),
                (_, VariableReference variable) => TryUnify(variable, x, unifier),
                (Function functionX, Function functionY) => TryUnify(functionX, functionY, unifier),
                _ => x.Equals(y), // TODO: only potential for equality is if they're both constants. Is it worth being explicit?
            };
        }

        private static bool TryUnify(VariableReference variable, Term other, LiteralUnifier unifier)
        {
            if (unifier.variableSubstitution.Bindings.TryGetValue(variable, out var value))
            {
                // The variable is already mapped to something - we need to make sure that the
                // mapping is consistent with the "other" value.
                return TryUnify(value, other, unifier);
            }
            else if (other is VariableReference otherVariable && unifier.variableSubstitution.Bindings.TryGetValue(otherVariable, out value))
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
                // This substitution is not in the source book, but is so that e.g. unifying Knows(John, X) and Knows(Y, Mother(Y)) will give { X / Mother(John) }, not { X / Mother(Y) }
                // Might be duplicated effort in the broader scheme of things, but time will tell.
                other = unifier.variableSubstitution.ApplyTo(other);
                unifier.variableSubstitution.AddBinding(variable, other);
                return true;
            }
        }

        private static bool TryUnify(Function x, Function y, LiteralUnifier unifier)
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
            // TODO-PERFORMANCE: this is very low-level code, so need to think about the GC impact when creating a bunch of these..
            // Caching? Mutability and pooling? Short-lived, so perhaps okay. Test me!
            var occursCheck = new OccursCheck(variable); 
            occursCheck.ApplyTo(term);
            return occursCheck.IsFound;
        }

        private class OccursCheck : SentenceTransformation
        {
            private readonly VariableReference variableReference;

            public OccursCheck(VariableReference variableReference) => this.variableReference = variableReference;

            public bool IsFound { get; private set; } = false;

            protected override Term ApplyTo(VariableReference variable)
            {
                if (variable.Equals(variableReference))
                {
                    // TODO-PERFORMANCE: For performance, should override everything and stop as soon as IsFound is true.
                    // And/or establish visitor pattern to make this easier.
                    IsFound = true;
                }

                return variable;
            }
        }

        private class VariableSubstitution : SentenceTransformation
        {
            private readonly Dictionary<VariableReference, Term> bindings = new Dictionary<VariableReference, Term>();

            public IReadOnlyDictionary<VariableReference, Term> Bindings => bindings;

            public void AddBinding(VariableReference variable, Term term) => bindings.Add(variable, term);

            protected override Term ApplyTo(VariableReference variable)
            {
                if (bindings.TryGetValue(variable, out var substitutedTerm))
                {
                    // Don't need base.ApplyTo because we apply this as we add each substitution.
                    return substitutedTerm;
                }

                return variable;
            }
        }
    }
}
