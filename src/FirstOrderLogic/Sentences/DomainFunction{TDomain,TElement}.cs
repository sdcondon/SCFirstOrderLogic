using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. Specifically,
    /// represents a function that refers to a particular element-valued method or property of elements of
    /// the domain.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class DomainFunction<TDomain, TElement> : Function<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public DomainFunction(MemberInfo memberInfo, IList<Term<TDomain, TElement>> arguments)
            : base(arguments)
        {
            Member = memberInfo;
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> instance for the logic behind this function.
        /// </summary>
        public MemberInfo Member { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is DomainFunction<TDomain, TElement> otherFunction)
                || !MemberInfoEqualityComparer.Instance.Equals(otherFunction.Member, Member)
                || otherFunction.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            // TODO: factor to base class..
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

            hashCode.Add(MemberInfoEqualityComparer.Instance.GetHashCode(Member));
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
