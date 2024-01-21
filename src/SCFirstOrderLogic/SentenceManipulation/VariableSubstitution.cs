// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Sentence transformation class that makes some substitutions for <see cref="VariableReference"/> instances.
    /// In addition to the <see cref="RecursiveSentenceTransformation.ApplyTo(Sentence)"/> method
    /// offered by the base class, this also offers an <see cref="ApplyTo(Literal)"/> method.
    /// </summary>
    // TODO-FEATURE/ROBUSTNESS: Make VariableSubstitution read-only (i.e. even internally),
    // and add MutableVariableSubstitution : VariableSubstitution.
    public class VariableSubstitution : RecursiveSentenceTransformation
    {
        private readonly Dictionary<VariableReference, Term> bindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableSubstitution"/> class that is empty.
        /// </summary>
        public VariableSubstitution()
        {
            bindings = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableSubstitution"/> class that uses a given set of bindings.
        /// </summary>
        /// <param name="bindings">The bindings to use.</param>
        public VariableSubstitution(IEnumerable<KeyValuePair<VariableReference, Term>> bindings)
        {
            this.bindings = new(bindings);
        }

        /// <summary>
        /// Gets the substitutions applied by this transformation.
        /// </summary>
        public IReadOnlyDictionary<VariableReference, Term> Bindings => bindings;

        /// <summary>
        /// Creates a copy of this substitution.
        /// </summary>
        /// <returns>A new <see cref="VariableSubstitution"/> instance with the same bindings as this one.</returns>
        public VariableSubstitution Clone() => new(bindings);

        /// <summary>
        /// Applies the substitution to a literal.
        /// </summary>
        /// <param name="literal">The literal to apply the substitution to.</param>
        /// <returns>The transformed version of the literal.</returns>
        public Literal ApplyTo(Literal literal)
        {
            return new Literal((Predicate)base.ApplyTo(literal.Predicate), literal.IsNegated);
        }

        /// <inheritdoc />
        public override Term ApplyTo(VariableReference variable)
        {
            if (Bindings.TryGetValue(variable, out var substitutedTerm))
            {
                // We need to call base.ApplyTo because we might be switching in a term
                // that itself is or contains variables that also need substituting.
                // TODO-ROBUSTNESS: In theory makes it possible to get us stuck in a loop, but that will
                // only happen as a result of bad consumer behaviour, not with e.g. unification
                // output. Don't want to add the performance burden of checking for this
                // every time, but could add a separate validation method to allow consumers
                // to opt-in to it.
                return base.ApplyTo(substitutedTerm);
            }

            return variable;
        }

        /// <inheritdoc />
        // TODO-BUG-ARGUABLE: no normalisation - {X/Y, Y/C} and {X/C, Y/C} give the same result when applied but are not viewed as the same
        public override bool Equals(object? obj)
        {
            if (obj is not VariableSubstitution otherSubstitution)
            {
                return false;
            }

            foreach (var kvp in bindings)
            {
                if (!otherSubstitution.bindings.TryGetValue(kvp.Key, out var otherValue) || !kvp.Value.Equals(otherValue))
                {
                    return false;
                }
            }

            foreach (var otherKvp in otherSubstitution.bindings)
            {
                if (!bindings.TryGetValue(otherKvp.Key, out var value) || !otherKvp.Value.Equals(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            HashCode hashCode = new();

            foreach (var kvp in bindings)
            {
                hashCode.Add(kvp.Key);
                hashCode.Add(kvp.Value);
            }

            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Adds a binding.
        /// </summary>
        /// <param name="variable">A reference to the variable to be substituted out.</param>
        /// <param name="term">The term to be substituted in.</param>
        internal void AddBinding(VariableReference variable, Term term)
        {
            bindings.Add(variable, term);
        }
    }
}
