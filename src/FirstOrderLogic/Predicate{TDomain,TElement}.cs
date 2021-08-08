using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>Predicate({term}, ..)</code>
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is a boolean-valued property or method call
    /// on a TElement, or a boolean-valued property or method call on TDomain.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Predicate<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        private readonly MemberInfo member;

        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Predicate(MemberInfo memberInfo, IList<Term<TDomain, TElement>> arguments)
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

        internal static new bool TryCreate(LambdaExpression lambda, out Sentence<TDomain, TElement> sentence)
        {
            if (lambda.Body.Type != typeof(bool))
            {
                sentence = null;
                return false;
            }

            if (lambda.Body is MemberExpression memberExpr && memberExpr.Expression != null) // Non-static field or property access
            {
                if (memberExpr.Expression == lambda.Parameters[0]) // i.e. is the domain parameter. Should try to find a cleaner way of doing this..
                {
                    // Boolean-valued property access on the domain parameter is interpreted as a ground predicate
                    sentence = new Predicate<TDomain, TElement>(memberExpr.Member, Array.Empty<Term<TDomain, TElement>>());
                    return true;
                }
                else if (Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(memberExpr.Expression), out var argument))
                {
                    // Boolean-valued property access on a term is interpreted as a unary predicate.
                    sentence = new Predicate<TDomain, TElement>(memberExpr.Member, new[] { argument });
                    return true;
                }
            }
            else if (lambda.Body is MethodCallExpression methodCallExpr && methodCallExpr.Object != null) // Non-static mmethod call
            {
                var arguments = new List<Term<TDomain, TElement>>();

                // If the method is not against on the domain, the instance it is operating on is the first arg of the predicate:
                if (methodCallExpr.Object != lambda.Parameters[0]) // i.e. is the domain parameter. Should try to find a cleaner way of doing this..
                {
                    if (!Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(methodCallExpr.Object), out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                // Each of the method's args should be interpretable as a term.
                foreach (var argExpr in methodCallExpr.Arguments)
                {
                    if (!Term<TDomain, TElement>.TryCreate(lambda.MakeSubLambda(argExpr), out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                sentence = new Predicate<TDomain, TElement>(methodCallExpr.Method, arguments);
                return true;
            }
            //// ... also to consider - certain things will fail the above but could be very sensibly interpreted
            //// as predicates. E.g. a Contains() call on a property that is a collection of TElements. Or indeed Any on
            //// an IEnumerable of them.

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Predicate<TDomain, TElement> otherPredicate)
                || !MemberInfoEqualityComparer.Instance.Equals(member, otherPredicate.member)
                || otherPredicate.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals(otherPredicate.Arguments[i]))
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
