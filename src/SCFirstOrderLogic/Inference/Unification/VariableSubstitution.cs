using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Sentence transformation class that makes some subtitutions for variable terms.
    /// Can be applied to <see cref="CNFLiteral"/>s as well as <see cref="Sentence"/>s.
    /// </summary>
    public class VariableSubstitution : SentenceTransformation
    {
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
            // should this complain if its not being applied to one of the literals it was created against?
            // or am I thinking about this wrong and we should always just be returning the unified literal?
            // wait and see..

            // TODO-PERFORMANCE / TODO-MAINTAINABILITY: Also, think about not using SentenceTransformation here - perhaps create CNFLiteralTransformation
            // (or just making VariableSubstitution contain the logic itself - creating a base class when there's only one implementation is needless complexity)
            var literalAsSentence = ApplyTo(literal.IsNegated ? (Sentence)new Negation(literal.Predicate) : literal.Predicate);
            return new CNFLiteral(literalAsSentence);
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
