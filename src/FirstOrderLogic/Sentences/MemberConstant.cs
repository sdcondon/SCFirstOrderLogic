using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    public class MemberConstant : Constant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="memberInfo">An expression for obtaining the value of the constant.</param>
        public MemberConstant(MemberInfo memberInfo) => Member = memberInfo;

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> instance for the logic behind this constant.
        /// </summary>
        public MemberInfo Member { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is MemberConstant otherMemberConstant && MemberInfoEqualityComparer.Instance.Equals(otherMemberConstant.Member, Member);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(MemberInfoEqualityComparer.Instance.GetHashCode(Member));
    }
}
