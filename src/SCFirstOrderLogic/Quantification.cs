using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a quantification sentence of first order logic.
    /// </summary>
    public abstract class Quantification : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Quantification"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        protected Quantification(VariableDeclaration variable, Sentence sentence) => (Variable, Sentence) = (variable, sentence);

        /// <summary>
        /// Gets the variable declared by this quantification.
        /// </summary>
        public VariableDeclaration Variable { get; }

        /// <summary>
        /// Gets the sentence that this quantification applies to.
        /// </summary>
        public Sentence Sentence { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Quantification quantification
                && Variable.Equals(quantification.Variable)
                && Sentence.Equals(quantification.Sentence);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Variable, Sentence);
    }
}
