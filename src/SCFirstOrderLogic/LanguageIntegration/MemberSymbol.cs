using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of a <see cref="Predicate"/>, <see cref="Function"/> or <see cref="Constant"/> symbol that refers to a particular class member.
    /// </summary>
    internal class MemberSymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberSymbol"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        public MemberSymbol(MemberInfo memberInfo) => MemberInfo = memberInfo;

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> to which this symbol refers.
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <inheritdoc />
        public override string ToString() => MemberInfo.Name;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is MemberSymbol otherMemberSymbol
                && MemberInfoEqualityComparer.Instance.Equals(MemberInfo, otherMemberSymbol.MemberInfo);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MemberInfoEqualityComparer.Instance.GetHashCode(this.MemberInfo);
        }
    }
}
