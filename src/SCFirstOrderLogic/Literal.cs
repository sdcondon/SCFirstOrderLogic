using SCFirstOrderLogic.SentenceFormatting;
using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Streamlined representation of a literal (i.e. a predicate or a negated predicate) of first-order logic.
    /// <para/>
    /// Note that this type is NOT a subtype of <see cref="Sentence"/>. To represent literals as <see cref="Sentence"/>s, 
    /// <see cref="SCFirstOrderLogic.Predicate"/> and <see cref="Negation"/> should be used as appropriate. This type is used within
    /// our <see cref="CNFClause"/> type, and may also be of use to consumers who want a streamlined literal representation.
    /// </summary>
    public sealed class Literal : IEquatable<Literal>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Literal"/> class.
        /// </summary>
        /// <param name="sentence">The literal, represented as a <see cref="Sentence"/> object. An exception will be thrown if it is neither a predicate nor a negated predicate.</param>
        public Literal(Sentence sentence)
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
        /// Initialises a new instance of the <see cref="Literal"/> class.
        /// </summary>
        /// <param name="predicate">The atomic sentence to which this literal refers.</param>
        /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
        public Literal(Predicate predicate, bool isNegated)
        {
            Predicate = predicate;
            IsNegated = isNegated;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Literal"/> class that is not negated.
        /// </summary>
        /// <param name="predicate">The atomic sentence to which this literal refers.</param>
        public Literal(Predicate predicate)
            : this(predicate, false)
        {
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
        public Literal Negate() => new(Predicate, !IsNegated);

        /// <summary>
        /// Returns a string that represents the current object.
        /// <para/>
        /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to format the literal.
        /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
        /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
        /// of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => new SentenceFormatter().Format(this);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Literal literal && Equals(literal);

        /// <inheritdoc />
        public bool Equals(Literal? other)
        {
            return other != null && other.Predicate.Equals(Predicate) && other.IsNegated.Equals(IsNegated);
        }

        /// <summary>
        /// Converts the literal to a <see cref="Sentence"/>
        /// </summary>
        /// <returns>A representation of this literal as a <see cref="Sentence"/>.</returns>
        public Sentence ToSentence()
        {
            return IsNegated ? new Negation(Predicate) : Predicate;
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Predicate, IsNegated);

        /// <summary>
        /// Defines the (implicit) conversion of a <see cref="SCFirstOrderLogic.Predicate"/> instance to a <see cref="Literal"/>. NB: This conversion is implicit because it is always valid and results in no loss of information.
        /// </summary>
        /// <param name="predicate">The predicate to convert.</param>
        public static implicit operator Literal(Predicate predicate) => new(predicate);
    }
}