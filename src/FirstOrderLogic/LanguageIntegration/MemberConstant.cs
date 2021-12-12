using System;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic. Specifically,
    /// represents a constant that refers to a particular element-valued method or property or parameterless method
    /// call on a class representing the domain.
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: Might ultimately be useful to make the Member.. classes generic in the same way as KnowledgeBase - for
    /// validation, as well as potential manipulation power.
    /// </remarks>
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
