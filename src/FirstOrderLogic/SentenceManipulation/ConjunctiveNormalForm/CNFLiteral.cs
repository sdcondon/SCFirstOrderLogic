// copied wholesale from LinqToKB.PredicateLogic..
#if false
using System;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic
{
    /// <summary>
    /// Representation of a literal of propositional logic. That is, an atomic sentence or a negated atomic sentence.
    /// </summary>
    /// <typeparam name="TModel">The type of model that this literal refers to.</typeparam>
    public class PLLiteral<TModel>
    {
        private bool isNegated;
        private PLAtomicSentence<TModel> atomicSentence;

        /// <summary>
        /// Initialises a new instance of the <see cref="PLLiteral{TModel}"/> class.
        /// </summary>
        /// <param name="lambda">The literal, represented as a lambda expression.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is a literal. If it were public we'd need to verify that.
        /// TODO-FEATURE: Perhaps could add this in future.
        /// </remarks>
        internal PLLiteral(Expression<Predicate<TModel>> lambda)
        {
            // TODO-ROBUSTNESS: Debug-only verification that it is actually a literal?
            Lambda = lambda; // Assumed to be a literal
            new LiteralConstructor(this).Visit(lambda.Body);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PLLiteral{TModel}"/> class.
        /// </summary>
        /// <param name="atomicSentence">The atomic sentence to which this literal refers.</param>
        /// <param name="isNegated">A value indicating whether the atomic sentence is negated.</param>
        /// <remarks>
        /// While there is nothing stopping this being public from a robustness perspective, there is as yet
        /// no easy way for consumers to instantiate an atomic sentence on its own - making it pointless for the
        /// moment.
        /// </remarks>
        internal PLLiteral(PLAtomicSentence<TModel> atomicSentence, bool isNegated)
        {
            this.atomicSentence = atomicSentence;
            this.isNegated = isNegated;

            if (isNegated)
            {
                Lambda = Expression.Lambda<Predicate<TModel>>(Expression.Not(atomicSentence.Lambda.Body), atomicSentence.Lambda.Parameters);
            }
            else
            {
                Lambda = atomicSentence.Lambda;
            }
        }

        /// <summary>
        /// Gets a representation of this literal as a lambda expression.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets a value indicating whether this literal is a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsNegated => isNegated;

        /// <summary>
        /// Gets a value indicating whether this literal is not a negation of the underlying atomic sentence.
        /// </summary>
        public bool IsPositive => !isNegated;

        /// <summary>
        /// Gets the underlying atomic sentence of this literal.
        /// </summary>
        public PLAtomicSentence<TModel> AtomicSentence => atomicSentence;

        /// <summary>
        /// Constructs and returns a literal that is the negation of this one.
        /// </summary>
        /// <returns>A literal that is the negation of this one.</returns>
        public PLLiteral<TModel> Negate() => new PLLiteral<TModel>(AtomicSentence, !IsNegated);

        /// <inheritdoc />
        public override string ToString() => $"{(IsNegated ? "¬" : string.Empty)}{AtomicSentence}";

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is PLLiteral<TModel> literal && atomicSentence.Equals(literal.atomicSentence) && isNegated == literal.isNegated;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(atomicSentence, isNegated);
        }

        private class LiteralConstructor : ExpressionVisitor
        {
            private readonly PLLiteral<TModel> owner;

            public LiteralConstructor(PLLiteral<TModel> owner) => this.owner = owner;

            public override Expression Visit(Expression node)
            {
                if (node is UnaryExpression not && not.NodeType == ExpressionType.Not)
                {
                    // Since we have assumed that the provided lambda is a literal,
                    // we know that a Not expression (if there is one) will be at the root..
                    owner.isNegated = true;
                    return base.Visit(node);
                }
                else
                {
                    owner.atomicSentence = new PLAtomicSentence<TModel>(Expression.Lambda<Predicate<TModel>>(node, owner.Lambda.Parameters));
                    return node; // no need to explore further, so not base.Visit
                }
            }
        }
    }
}
#endif