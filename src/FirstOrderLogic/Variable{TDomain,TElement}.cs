using System;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a variable term within a sentence of first order logic. These are declared in quantifier sentences.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Variable<TDomain, TElement> : Term<TDomain, TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Variable{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public Variable(string name) => Name = name;

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        internal static bool TryCreate(LambdaExpression expression, out Variable<TDomain, TElement> term)
        {
            if (expression.Body is ParameterExpression parameterExpr
                && typeof(TElement).IsAssignableFrom(parameterExpr.Type))
            {
                term = new Variable<TDomain, TElement>(parameterExpr.Name);
                return true;
            }

            term = null;
            return false;
        }

        internal static new bool TryCreate(LambdaExpression expression, out Term<TDomain, TElement> term)
        {
            var returnValue = TryCreate(expression, out Variable<TDomain, TElement> variableTerm);
            term = variableTerm;
            return returnValue;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Variable<TDomain, TElement> otherVariable && Name.Equals(otherVariable.Name); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
