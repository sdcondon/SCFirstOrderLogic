using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>Predicate({term}, ..)</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is a boolean-valued property or method call
    /// on a TElement, or a boolean-valued property or method call on TDomain.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class MemberPredicate<TDomain, TElement> : Predicate<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public MemberPredicate(MemberInfo memberInfo, IList<Term<TDomain, TElement>> arguments)
            : base(arguments)
        {
            Member = memberInfo; // TODO: This is public - so should probably validate that its boolean valued and that the arguments match it..
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> instance for the logic behind this predicate.
        /// </summary>
        public MemberInfo Member { get; }

        /// <inheritdoc />
        public override bool SymbolEquals(Predicate<TDomain, TElement> other)
        {
            return other is MemberPredicate<TDomain, TElement> otherMemberPredicate
                && MemberInfoEqualityComparer.Instance.Equals(Member, otherMemberPredicate.Member);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is MemberPredicate<TDomain, TElement> otherPredicate)
                || !MemberInfoEqualityComparer.Instance.Equals(Member, otherPredicate.Member)
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

            hashCode.Add(MemberInfoEqualityComparer.Instance.GetHashCode(Member));
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
