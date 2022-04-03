using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of the symbol of a <see cref="Constant"/> that refers to a particular element-valued
    /// method or property or parameterless method call on a class representing the domain.
    /// </summary>
    /// <remarks>
    /// TODO-ZZZ/MINOR-FUNCTIONALITY: Might ultimately be useful to make the Member..Symbol classes generic in the same way as ILinqKnowledgeBase - for
    /// validation, as well as potential manipulation power.
    /// </remarks>
    public class MemberConstantSymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstantSymbol"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        public MemberConstantSymbol(MemberInfo memberInfo) => MemberInfo = memberInfo;

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> to which this symbol refers.
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <inheritdoc />
        public override string ToString() => MemberInfo.Name;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is MemberConstantSymbol otherMemberConstantSymbol
                && MemberInfoEqualityComparer.Instance.Equals(MemberInfo, otherMemberConstantSymbol.MemberInfo);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MemberInfoEqualityComparer.Instance.GetHashCode(this.MemberInfo);
        }
    }
}
