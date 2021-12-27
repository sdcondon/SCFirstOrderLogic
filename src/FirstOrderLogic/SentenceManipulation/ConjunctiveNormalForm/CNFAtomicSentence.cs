using System;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of an atomic sentence (i.e. a predicate) of first-order logic within a sentence in conjunctive normal form.
    /// </summary>
    /// <remarks>
    /// NB: Yes, atomic sentences are a meaningful notion regardless of CNF, but we only use THIS type within our CNF representation. Hence this type being called CNFAtomicSentence and residing in this namespace.
    /// </remarks>
    public class CNFAtomicSentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CNFAtomicSentence"/> class.
        /// </summary>
        /// <param name="sentence">The atomic sentence.</param>
        public CNFAtomicSentence(Sentence sentence)
        {
            Sentence = sentence switch
            {
                Predicate predicate => predicate,
                Equality equality => equality,
                _ => throw new ArgumentException($"{sentence} is not an atomic sentence", nameof(sentence)),
            };
        }

        /// <summary>
        /// Gets the <see cref="Sentence"/> instance that is equivalent to this object.
        /// </summary>
        public Sentence Sentence { get; }

        /// <inheritdoc />
        public override string ToString() => Sentence.ToString();

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