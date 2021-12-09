using System;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an negation sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>¬{sentence}</code>
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
