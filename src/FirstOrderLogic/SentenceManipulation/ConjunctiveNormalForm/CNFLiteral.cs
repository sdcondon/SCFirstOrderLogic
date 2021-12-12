using System;

namespace LinqToKB.FirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of a literal of first-order logic. That is, an atomic sentence or a negated atomic sentence.
    /// </summary>
    /// <remarks>
    /// Yes, literals are a meaningful notion regardless of CNF, but we only use THIS type within our CNF representation. For now then, its called CNFLiteral and resides in this namespace.
    /// </remarks>
    public class CNFLiteral
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="lambda">The literal, represented as a lambda expression.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is a literal. If it were public we'd need to verify that.
        /// TODO-FEATURE: Perhaps could add this in future.
        /// </remarks>
        internal CNFLiteral(Sentence sentence)
        {
            Sentence = sentence;

            if (sentence is Negation negation)
            {
                IsNegated = true;
                sentence = negation.Sentence;
            }

            // NB: Will throw its not actually an atomic sentence.
            AtomicSentence = new CNFAtomicSentence(sentence);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="atomicSentence">The atomic sentence to which this literal refers.</param>
        /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
        /// <remarks>
        /// While there is nothing stopping this being public from a robustness perspective, there is as yet
        /// no easy way for consumers to instantiate an atomic sentence on its own - making it pointless for the
        /// moment.
        /// </remarks>
        internal CNFLiteral(CNFAtomicSentence atomicSentence, bool isNegated)
        {
            AtomicSentence = atomicSentence;
            IsNegated = isNegated;

            if (isNegated)
            {
                Sentence = new Negation(atomicSentence.Sentence);
            }
            else
            {
                Sentence = atomicSentence.Sentence;
            }
        }

        /// <summary>
        /// Gets the actual <see cref="Sentence"/> that underlies this representation.
        /// </summary>
        public Sentence Sentence { get; }

        /// <summary>
        /// Gets a value indicating whether this literal is a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsNegated { get; }

        /// <summary>
        /// Gets a value indicating whether this literal is not a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsPositive => !IsNegated;

        /// <summary>
        /// Gets the underlying atomic sentence of this literal.
        /// </summary>
        public CNFAtomicSentence AtomicSentence { get; }

        /// <summary>
        /// Constructs and returns a literal that is the negation of this one.
        /// </summary>
        /// <returns>A literal that is the negation of this one.</returns>
        public CNFLiteral Negate() => new CNFLiteral(AtomicSentence, !IsNegated);

        /// <inheritdoc />
        public override string ToString() => Sentence.ToString();

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is CNFLiteral literal && literal.Sentence.Equals(Sentence);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence);
    }
}