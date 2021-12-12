// Copied wholesale from LinqToKB.PropositionalLogic
#if false
using System;
using System.Linq.Expressions;

namespace LinqToKB.PropositionalLogic
{
    /// <summary>
    /// Representation of an atomic sentence of propositional logic.
    /// </summary>
    /// <typeparam name="TModel">The type of model that this atomic sentence refers to.</typeparam>
    public class PLAtomicSentence<TModel>
    {
        private object equalitySurrogate;
        private string symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="PLAtomicSentence{TModel}"/> class.
        /// </summary>
        /// <param name="lambda">The atomic sentence, represented as a lambda expression.</param>
        /// <remarks>
        /// NB: Internal because it makes the assumption that the lambda is an atomic sentence. If it were public we'd need to verify that.
        /// TODO-FEATURE: Perhaps could add this in future.
        /// </remarks>
        internal PLAtomicSentence(Expression<Predicate<TModel>> lambda)
        {
            // TODO-ROBUSTNESS: Debug-only verification that it is actually an atomic sentence?
            Lambda = lambda; // Assumed to be an atomic sentence

            new AtomicSentenceConstructor(this).Visit(lambda.Body);
        }

        /// <summary>
        /// Gets a representation of this atomic sentence as a lambda expression.
        /// </summary>
        public Expression<Predicate<TModel>> Lambda { get; }

        /// <summary>
        /// Gets a string representation of this atomic sentence.
        /// </summary>
        public string Symbol => symbol;

        //// TODO-FEATURE: Introducing some public methods to create atomic sentences directly
        //// would in turn facilitate being able to make some of the internal ctors for PLLiteral et
        //// al public too, increasing the functionality of the library.
        //// Perhaps some static factory methods (e.g. Property(PropertyInfo))?

        /// <inheritdoc />
        public override string ToString() => symbol;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is PLAtomicSentence<TModel> atomicSentence && equalitySurrogate.Equals(atomicSentence.equalitySurrogate);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return equalitySurrogate.GetHashCode();
        }

        // TODO-MAINTAINABILITY: no need for an expression visitor unless we end up making the ctor public and verifying that it is actually an atomic sentence,
        // because all we do at the mo is stop at the root node..
        private class AtomicSentenceConstructor : ExpressionVisitor
        {
            private readonly PLAtomicSentence<TModel> owner;

            public AtomicSentenceConstructor(PLAtomicSentence<TModel> owner) => this.owner = owner;

            public override Expression Visit(Expression node)
            {
                (owner.equalitySurrogate, owner.symbol) = node switch
                {
                    // TODO-ROBUSTNESS: Need more here - ideally, it should be exhaustive.. Really need
                    // comprehensively defined strategy for exactly how lambdas are interpreted as PL sentences.
                    // TODO-PERFORMANCE: Value tuple for equality surrogate - so boxing. Doesn't happen a lot though, so probably okay. Keep an eye on what the surrogate looks like for other expression types as they are added..
                    MemberExpression memberExpr => ((memberExpr.Member.Module, memberExpr.Member.MetadataToken), memberExpr.Member.Name),

                    _ => throw new ArgumentException($"Node type {node.GetType()} ({node.NodeType}) not supported", nameof(node)),
                };

                return node; // no need to explore further, so not base.Visit
            }
        }
    }
}
#endif