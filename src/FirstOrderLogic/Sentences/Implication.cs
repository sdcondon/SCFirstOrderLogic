using System;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a material implication sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{sentence} ⇒ {sentence}</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>Operators.If({expression}, {expression})</code>
    /// (Consumers are encouraged to include <c>using static LinqToKB.FirstOrderLogic.Symbols;</c> to make this a little shorter)
    /// </summary>
    public class Implication : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Implication"/> class.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence.</param>
        /// <param name="consequent">The consequent sentence.</param>
        public Implication(Sentence antecedent, Sentence consequent) => (Antecedent, Consequent) = (antecedent, consequent);

        /// <summary>
        /// Gets the antecedent sentence.
        /// </summary>
        public Sentence Antecedent { get; }

        /// <summary>
        /// Gets the consequent sentence.
        /// </summary>
        public Sentence Consequent { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Implication otherImplication && Antecedent.Equals(otherImplication.Antecedent);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Antecedent, Consequent);
    }
}
