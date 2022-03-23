using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a universal quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∀ {variable}, {sentence}</code>
    /// </summary>
    public sealed class UniversalQuantification : Quantification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalQuantification"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public UniversalQuantification(VariableDeclaration variable, Sentence sentence)
            : base(variable, sentence)
        {
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is UniversalQuantification universalQuantification && base.Equals(universalQuantification);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Variable, Sentence);
    }
}
