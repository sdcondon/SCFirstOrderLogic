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
    public class MemberFunction : Function
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFunction"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this function.</param>
        public MemberFunction(MemberInfo memberInfo, IList<Term> arguments)
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
            if (!(obj is MemberFunction otherFunction)
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
