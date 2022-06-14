using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Sentence transformation class that makes some substitutions for variable terms.
    /// In addition to the <see cref="SentenceTransformation.ApplyTo(Sentence)"/> method
    /// offered by the base class, this also offers an <see cref="ApplyTo(CNFLiteral)"/> method.
    /// </summary>
    public class VariableSubstitution : SentenceTransformation
    {
        private readonly Dictionary<VariableReference, Term> bindings = new();

        /// <summary>
        /// Gets the substitions applied by this transformation.
        /// </summary>
        public IReadOnlyDictionary<VariableReference, Term> Bindings => bindings;

        /// <summary>
        /// Adds a binding.
        /// </summary>
        /// <param name="variable">A reference to the variable to be substituted out.</param>
        /// <param name="term">The term to be substituted in.</param>
        internal void AddBinding(VariableReference variable, Term term)
        {
            bindings.Add(variable, term);
        }

        /// <summary>
        /// Applies the unifier to a literal.
        /// </summary>
        /// <param name="literal">The literal to apply the unifier to.</param>
        /// <returns>The unified version of the literal.</returns>
        public CNFLiteral ApplyTo(CNFLiteral literal)
        {
            // TODO-PERFORMANCE vs TODO-MAINTAINABILITY: Avoidable converting to and from Sentence here.
            // How much faster would it be to just make this class contain the logic itself - test me!
            return new CNFLiteral(ApplyTo(literal.ToSentence()));
        }

        protected override Term ApplyTo(VariableReference variable)
        {
            if (Bindings.TryGetValue(variable, out var substitutedTerm))
            {
                // Don't need base.ApplyTo because we apply this as we add each substitution.
                return substitutedTerm;
            }

            return variable;
        }
    }
}
