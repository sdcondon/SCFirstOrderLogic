using System;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of an atomic sentence of first-order logic - a predicate (or equality).
    /// </summary>
    /// <remarks>
    /// Yes, atomic sentences are a meaningful notion regardless of CNF, but we only use THIS type within our CNF representation. For now then, its called CNFLiteral and resides in this namespace.
    /// </remarks>
    public class CNFAtomicSentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CNFAtomicSentence"/> class.
        /// </summary>
        /// <param name="sentence">The atomic sentence.</param>
        internal CNFAtomicSentence(Sentence sentence)
        {
            Sentence = sentence switch
            {
                Predicate predicate => predicate,
                Equality equality => equality,
                _ => throw new ArgumentException($"{sentence} is not an atomic sentence", nameof(sentence)),
            };
        }

        /// <summary>
        /// Gets the actual <see cref="Sentence"/> that underlies this representation.
        /// </summary>
        public Sentence Sentence { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CNFAtomicSentence atomicSentence && atomicSentence.Sentence.Equals(Sentence);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Sentence);
        }
    }
}