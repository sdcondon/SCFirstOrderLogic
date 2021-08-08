using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Function<TDomain, TElement> : Term<TDomain, TElement>
    {
        private readonly MemberInfo member;

        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Function(MemberInfo memberInfo, IList<Term<TDomain, TElement>> arguments)
        {
            member = memberInfo;
            Arguments = new ReadOnlyCollection<Term<TDomain, TElement>>(arguments);
        }

        /// <summary>
        /// Gets the name of this predicate.
        /// </summary>
        public string Name => member.Name;

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term<TDomain, TElement>> Arguments { get; }

        internal static new bool TryCreate(LambdaExpression lambda, out Term<TDomain, TElement> term)
        {
            // NB: Here we verify that the value of the function is a domain element..
            if (typeof(TElement).IsAssignableFrom(lambda.Body.Type))
            {
                if (lambda.Body is MemberExpression memberExpr
                    && Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(memberExpr.Expression), out var argument))
                {
                    // TElement-valued property access is interpreted as a unary function.
                    term = new Function<TDomain, TElement>(memberExpr.Member, new[] { argument });
                    return true;
                }
                else if (lambda.Body is MethodCallExpression methodCallExpr
                    && (methodCallExpr.Object != null || methodCallExpr.Arguments.Count > 0)) // NB: There must be at least one arg - otherwise its a constant, not a function..
                {
                    var arguments = new List<Term<TDomain, TElement>>();

                    // If the method is non-static, the instance it is operating on is the first arg of the predicate:
                    if (methodCallExpr.Object != null)
                    {
                        if (!Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(methodCallExpr.Object), out var arg))
                        {
                            term = null;
                            return false;
                        }

                        arguments.Add(arg);
                    }

                    foreach (var argExpr in methodCallExpr.Arguments)
                    {
                        if (!Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(argExpr), out var arg))
                        {
                            term = null;
                            return false;
                        }

                        arguments.Add(arg);
                    }

                    term = new Function<TDomain, TElement>(methodCallExpr.Method, arguments);
                    return true;
                }
            }

            term = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Function<TDomain, TElement> otherFunction)
                || !MemberInfoEqualityComparer.Instance.Equals(otherFunction.member, member)
                || otherFunction.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals(otherFunction.Arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(MemberInfoEqualityComparer.Instance.GetHashCode(member));
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
