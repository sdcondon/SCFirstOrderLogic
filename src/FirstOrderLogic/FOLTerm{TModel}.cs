using System;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic
{
    public abstract class FOLTerm<TModel> // TODO: Will almost certainly need (possibly a derived class with - though be careful of boxing, i guess) a second generic type param - the type of the term..
    {
        internal static bool TryCreate(LambdaExpression lambda, out FOLTerm<TModel> sentence)
        {
            return FOLFunctionTerm<TModel>.TryCreate(lambda, out sentence)
                || FOLConstantTerm<TModel>.TryCreate(lambda, out sentence)
                || FOLVariableTerm<TModel>.TryCreate(lambda, out sentence);
        }
    }

    public class FOLFunctionTerm<TModel> : FOLTerm<TModel>
    {
        internal static new bool TryCreate(LambdaExpression lambda, out FOLTerm<TModel> term)
        {
            // TODO!
            term = null;
            return false;
        }
    }

    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class FOLConstantTerm<TModel> : FOLTerm<TModel>
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

        internal static new bool TryCreate(LambdaExpression lambda, out FOLTerm<TModel> term)
        {
            if (lambda.Body is ConstantExpression constantExpr)
            {
                term = new FOLConstantTerm<TModel>(constantExpr.Value);
                return true;
            }

            term = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is FOLConstantTerm<TModel> otherConstant && Value.Equals(otherConstant.Value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }

    /// <summary>
    /// Representation of a variable term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class FOLVariableTerm<TModel> : FOLTerm<TModel>
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

        internal static bool TryCreate(LambdaExpression expression, out FOLVariableTerm<TModel> term)
        {
            if (expression.Body is ParameterExpression parameterExpr)
            {
                term = new FOLVariableTerm<TModel>(parameterExpr.Name);
                return true;
            }

            term = null;
            return false;
        }

        internal static new bool TryCreate(LambdaExpression expression, out FOLTerm<TModel> term)
        {
            var returnValue = TryCreate(expression, out FOLVariableTerm<TModel> variableTerm);
            term = variableTerm;
            return returnValue;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is FOLVariableTerm<TModel> otherVariable && Name.Equals(otherVariable.Name); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
