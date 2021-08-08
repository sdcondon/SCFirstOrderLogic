using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Constant<TDomain, TElement> : Term<TDomain, TElement>
    {
        private readonly MemberInfo member;

        /// <summary>
        /// Initializes a new instance of the <see cref="Constant{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="valueExpr">An expression for obtaining the value of the constant.</param>
        public Constant(MemberInfo memberInfo) => member = memberInfo;

        /// <summary>
        /// Gets the name of the constant.
        /// </summary>
        public string Name => member.Name;

        internal static new bool TryCreate(LambdaExpression lambda, out Term<TDomain, TElement> term)
        {
            if (typeof(TElement).IsAssignableFrom(lambda.Body.Type)) // Constants must be elements of the domain
            {
                if (lambda.Body is MemberExpression memberExpr
                    && typeof(TDomain).IsAssignableFrom(memberExpr.Expression.Type)) // TODO-ROBUSTNESS: Do we actually need to check if its accessing the domain-valued param (think of weird situations where its a domain-valued prop of an element or somat)..
                {
                    // TElement-valued property access of the domain is interpreted as a constant.
                    term = new Constant<TDomain, TElement>(memberExpr.Member);
                    return true;
                }
                else if (lambda.Body is MethodCallExpression methodCallExpr
                    && typeof(TDomain).IsAssignableFrom(methodCallExpr.Object.Type) // TODO-ROBUSTNESS: Do we actually need to check if its accessing the domain-valued param (think of weird situations where its a domain-valued prop of an element or somat)..
                    && methodCallExpr.Arguments.Count == 0)
                {
                    // TElement-valued parameterless method call of the domain is interpreted as a constant.
                    term = new Constant<TDomain, TElement>(methodCallExpr.Method);
                    return true;
                }
            }

            // TODO-USABILITY: Perhaps we should allow constants here too? For cases where the domain is of basic types, it seems
            // silly to require d => d.Hello rather than d => "Hello". For user provided types is of less value because
            // it requires an instantiation of the constant (where otherwise just working with interfaces is fine), but
            // as far as I can see there's no reason (aside from equality et al - probably solvable) to actually forbid it..?

            term = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Constant<TDomain, TElement> otherConstant && MemberInfoEqualityComparer.Instance.Equals(otherConstant.member, member);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(MemberInfoEqualityComparer.Instance.GetHashCode(member));
    }
}
