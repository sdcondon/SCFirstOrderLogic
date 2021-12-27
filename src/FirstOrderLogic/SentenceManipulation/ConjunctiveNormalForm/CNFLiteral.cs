using System;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of a literal (i.e. an atomic sentence or a negated atomic sentence) of first-order logic within a sentence in conjunctive normal form.
    /// </summary>
    /// <remarks>
    /// NB: Yes, literals are a meaningful notion regardless of CNF, but we only use THIS type within our CNF representation. Hence this type being called CNFLiteral and residing in this namespace.
    /// </remarks>
    public class CNFLiteral
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="lambda">The literal, represented as a lambda expression.</param>
        public CNFLiteral(Sentence sentence)
        {
            Sentence = sentence;

            if (sentence is Negation negation)
            {
                IsNegated = true;
                sentence = negation.Sentence;
            }

            // NB: this Will throw its not actually an atomic sentence.
            AtomicSentence = new CNFAtomicSentence(sentence);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="atomicSentence">The atomic sentence to which this literal refers.</param>
        /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
        public CNFLiteral(CNFAtomicSentence atomicSentence, bool isNegated)
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
        /// Gets the <see cref="Sentence"/> instance that is equivalent to this object.
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