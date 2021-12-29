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
        /// <param name="sentence">The literal, represented as a <see cref="Sentence"/> object.</param>
        public CNFLiteral(Sentence sentence)
        {
            Sentence = sentence;

            if (sentence is Negation negation)
            {
                IsNegated = true;
                sentence = negation.Sentence;
            }

            if (sentence is Predicate predicate)
            {
                Predicate = predicate;
            }
            else
            {
                throw new ArgumentException($"Provided sentence must be either a predicate or a negated predicate. {sentence} is neither.");
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFLiteral"/> class.
        /// </summary>
        /// <param name="predicate">The atomic sentence to which this literal refers.</param>
        /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
        public CNFLiteral(Predicate predicate, bool isNegated)
        {
            Predicate = predicate;
            IsNegated = isNegated;

            if (isNegated)
            {
                Sentence = new Negation(predicate);
            }
            else
            {
                Sentence = predicate;
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
        public Predicate Predicate { get; }

        /// <summary>
        /// Constructs and returns a literal that is the negation of this one.
        /// </summary>
        /// <returns>A literal that is the negation of this one.</returns>
        public CNFLiteral Negate() => new CNFLiteral(Predicate, !IsNegated);

        /// <inheritdoc />
        public override string ToString() => Sentence.ToString();

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is CNFLiteral literal && literal.Sentence.Equals(Sentence); // TODO: Think about if its better to use the predicate and isnegated here?

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Sentence); // TODO: Think about if its better to use the predicate and isnegated here?
    }
}