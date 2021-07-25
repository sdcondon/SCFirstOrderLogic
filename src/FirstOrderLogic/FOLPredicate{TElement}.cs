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
    /// In C#, the equivalent expression acting on the domain (as well as any relevant variables and constants) is a boolean-valued property or method call,
    /// or a boolean-valued constant (a constant in the lambda, that is - could be a valued captured in a closure).
    /// </summary>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    /// <remarks>
    /// Lots of things to reconsider about this class:
    /// <list type="bullet">
    /// <item>
    /// Arguments collection instead of multiple versions of this class for type safety. Figure this way is PROBABLY fine since the usual method for instantiating
    /// these will be from lambdas - so lots of type safety isn't actually that important. Maybe.
    /// </item>
    /// <item>
    /// Note how "ground" predicates are dealt with by looking for bool value constants (which could come from e.g. closures). This is perhaps not as good as allowing
    /// the domain type to declare them - which would be possible if instead of TElement, we had TDomain where TDomain : IEnumerable&lt;TElement&gt;. Which is obviously a huge change..
    /// </item>
    /// <item>
    /// I'm not sure about the need to distinguish between Predicates and boolean-valued functions (there's no reason why booleans can't be part of the domain.. I think).
    /// Maybe I'm overthinking it, but it's something I'm tempted to come back to.
    /// </item>
    /// </list>
    /// </remarks>
    public class FOLPredicate<TElement> : FOLAtomicSentence<TElement>
    {
        private readonly (Module Module, int MetadataToken) member;

        /// <summary>
        /// Initializes a new instance of the <see cref="FOLPredicate{TElement}"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public FOLPredicate(MemberInfo memberInfo, IList<FOLTerm<TElement>> arguments)
        {
            member = (memberInfo.Module, memberInfo.MetadataToken);
            Name = memberInfo.Name;
            Arguments = new ReadOnlyCollection<FOLTerm<TElement>>(arguments);
        }

        /// <summary>
        /// Gets the name of this predicate.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<FOLTerm<TElement>> Arguments { get; }

        internal static new bool TryCreate(Expression<Predicate<IEnumerable<TElement>>> lambda, out FOLSentence<TElement> sentence)
        {
            if (lambda.Body is MemberExpression memberExpr && memberExpr.Type == typeof(bool)
                && FOLTerm<TElement>.TryCreate(lambda.MakeSubLambda(memberExpr.Expression), out var argument))
            {
                // Boolean-valued property access is interpreted as a unary predicate.
                sentence = new FOLPredicate<TElement>(memberExpr.Member, new[] { argument });
                return true;
            }
            else if (lambda.Body is MethodCallExpression methodCallExpr && methodCallExpr.Type == typeof(bool))
            {
                var arguments = new List<FOLTerm<TElement>>();

                // If the method is non-static, the object it is operating on is the first arg of the predicate:
                if (methodCallExpr.Object != null)
                {
                    if (!FOLTerm<TElement>.TryCreate(lambda.MakeSubLambda(methodCallExpr.Object), out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                foreach (var argExpr in methodCallExpr.Arguments)
                {
                    if (!FOLTerm<TElement>.TryCreate(lambda.MakeSubLambda(argExpr), out var arg))
                    {
                        sentence = null;
                        return false;
                    }

                    arguments.Add(arg);
                }

                sentence = new FOLPredicate<TElement>(methodCallExpr.Method, arguments);
                return true;
            }
            else if (lambda.Body is ConstantExpression constantExpr && constantExpr.Type == typeof(bool))
            {
                // Hacky way of allowing for ground predicates.. Might be non-workable - plan B is to use <TDomain, TElement>
                // where TDomain : IEnumerable and look for bool-valued property access against TDomain here.
            }
            // ... also to consider - certain methods will fail the above but could be very sensibly interpreted
            // as predicates. E.g. Contains on a property that is a collection of TElements..

            sentence = null;
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is FOLPredicate<TElement> otherPredicate) || otherPredicate.member != member || otherPredicate.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Arguments[i].Equals(otherPredicate.Arguments[i]))
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
            hashCode.Add(member);

            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
