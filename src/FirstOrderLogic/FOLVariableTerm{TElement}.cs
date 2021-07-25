using System;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a variable term within a sentence of first order logic. These are declared in quantifier sentences.
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class FOLVariableTerm<TElement> : FOLTerm<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLVariableTerm{TModel}"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public FOLVariableTerm(string name) => Name = name;

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        internal static bool TryCreate(LambdaExpression expression, out FOLVariableTerm<TElement> term)
        {
            if (expression.Body is ParameterExpression parameterExpr)
            {
                // TODO-ROBUSTNESS: We possibly need to verify that the value is assignable to (or equal to?) TElement. Otherwise
                // we might e.g. mistakenly interpret something like "x.GetType() == typeof(ParticularType)" as an
                // FOLEquality when its actually more likely to be intended as a predicate?
                term = new FOLVariableTerm<TElement>(parameterExpr.Name);
                return true;
            }

            term = null;
            return false;
        }

        internal static new bool TryCreate(LambdaExpression expression, out FOLTerm<TElement> term)
        {
            var returnValue = TryCreate(expression, out FOLVariableTerm<TElement> variableTerm);
            term = variableTerm;
            return returnValue;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is FOLVariableTerm<TElement> otherVariable && Name.Equals(otherVariable.Name); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
