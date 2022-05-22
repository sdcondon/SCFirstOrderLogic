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
        // TODO-MAINTAINABILITY: This was lazy - I don't like non-private fields. Perhaps add an internal AddBinding method instead?
        internal readonly Dictionary<VariableReference, Term> bindings = new Dictionary<VariableReference, Term>();

        /// <summary>
        /// Gets the substitions applied by this transformation.
        /// </summary>
        public IReadOnlyDictionary<VariableReference, Term> Bindings => bindings;

        /// <summary>
        /// Applies the unifier to a literal.
        /// </summary>
        /// <param name="literal">The literal to apply the unifier to.</param>
        /// <returns>The unified version of the literal.</returns>
        public CNFLiteral ApplyTo(CNFLiteral literal)
        {
            // TODO-PERFORMANCE / TODO-MAINTAINABILITY: Think about not using SentenceTransformation here - perhaps create CNFLiteralTransformation
            // (or just make VariableSubstitution contain the logic itself - creating a base class when there's only one implementation is needless complexity)
            // TODO-MAINTAINABILITY: Logic for conversion of a CNFLiteral back to a Sentence really belongs in the CNFLiteral class..
            var literalAsSentence = literal.IsNegated ? (Sentence)new Negation(literal.Predicate) : literal.Predicate;
            return new CNFLiteral(ApplyTo(literalAsSentence));
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
