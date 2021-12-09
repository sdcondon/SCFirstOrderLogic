using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>Predicate({term}, ..)</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is a boolean-valued property
    /// or method call on an element object, or a boolean-valued property or method call on a domain object.
    /// </summary>
    public class MemberPredicate : Predicate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPredicate"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public MemberPredicate(MemberInfo memberInfo, params Term[] arguments)
            : this(memberInfo, (IList<Term>)arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPredicate"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public MemberPredicate(MemberInfo memberInfo, IList<Term> arguments)
            : base(arguments)
        {
            Member = memberInfo; // TODO: This is public - so should probably validate that its boolean valued and that the arguments match it..
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> instance for the logic behind this predicate.
        /// </summary>
        public MemberInfo Member { get; }

        /// <inheritdoc />
        public override bool SymbolEquals(Predicate other)
        {
            return other is MemberPredicate otherMemberPredicate
                && MemberInfoEqualityComparer.Instance.Equals(Member, otherMemberPredicate.Member);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is MemberPredicate otherPredicate)
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
