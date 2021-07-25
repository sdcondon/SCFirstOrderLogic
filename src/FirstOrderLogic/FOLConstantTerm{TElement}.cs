using System;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLConstantTerm<TElement> : FOLTerm<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLConstantTerm{TModel}"/> class.
        /// </summary>
        /// <param name="name">The value of the constant.</param>
        public FOLConstantTerm(object value) => Value = value;

        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        public object Value { get; }

        internal static new bool TryCreate(LambdaExpression lambda, out FOLTerm<TElement> term)
        {
            if (lambda.Body is ConstantExpression constantExpr)
            {
                // TODO-ROBUSTNESS: We possibly need to verify that the value is assignable to (or equal to?) TElement. Otherwise
                // we might e.g. mistakenly interpret something like "x.GetType() == typeof(Foo)" as an
                // FOLEquality when its actually more likely to be intended as a predicate?
                term = new FOLConstantTerm<TElement>(constantExpr.Value);
                return true;
            }

            term = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is FOLConstantTerm<TElement> otherConstant && Value.Equals(otherConstant.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}
