using System;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an negation sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>¬{sentence}</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>!{expression}</code>
    /// We also interpret <c>!=</c> as a negation of an equality.
    /// </summary>
    public class Negation : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Negation"/> class.
        /// </summary>
        /// <param name="sentence">The sentence that is negated.</param>
        public Negation(Sentence sentence) => Sentence = sentence;

        /// <summary>
        /// Gets the sentence that is negated.
        /// </summary>
        public Sentence Sentence { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Negation negation && Sentence.Equals(negation.Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }
}
