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
        /// <param name="sentence">The literal, represented as a <see cref="Sentence"/> object. An exception will be thrown if it is neither a predicate nor a negated predicate.</param>
        public CNFLiteral(Sentence sentence)
        {
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
                throw new ArgumentException($"Provided sentence must be either a predicate or a negated predicate. {sentence} is neither.", nameof(sentence));
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
        }

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
        public override string ToString() => $"{(IsNegated ? "¬" : "")}{Predicate}";

        /// <inheritdoc />
        /// TODO: Explain this choice of equality (and hashcode) implementation.
        public override bool Equals(object obj)
        {
            return obj is CNFLiteral literal
                && literal.Predicate.Equals(Predicate)
                && literal.IsNegated.Equals(IsNegated);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Predicate, IsNegated);

        /// <summary>
        /// Defines the (explicit) conversion of a <see cref="Sentence"/> instance to a <see cref="CNFLiteral"/>. NB: This conversion is explicit because it can fail (if the sentence isn't actually a literal).
        /// </summary>
        /// <param name="sentence">The sentence to convert.</param>
        public static explicit operator CNFLiteral(Sentence sentence)
        {
            try
            {
                return new CNFLiteral(sentence);
            }
            catch (ArgumentException e)
            {
                throw new InvalidCastException($"To be converted to a literal, sentences must be either a predicate or a negated predicate. {sentence} is neither.", e);
            }
        }

        /// <summary>
        /// Defines the (implicit) conversion of a <see cref="Predicate"/> instance to a <see cref="CNFLiteral"/>. NB: This conversion is implicit because it is always valid.
        /// </summary>
        /// <param name="sentence">The predicate to convert.</param>
        public static implicit operator CNFLiteral(Predicate predicate) => new CNFLiteral(predicate);
    }
}