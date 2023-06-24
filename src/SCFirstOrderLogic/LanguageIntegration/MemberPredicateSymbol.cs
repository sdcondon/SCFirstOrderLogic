// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of the symbol of a <see cref="Predicate"/>  that refers to a particular boolean-valued method or
    /// property of elements of the domain (or the domain itself, in the case of ground predicates).
    /// </summary>
    /// <remarks>
    /// Might ultimately be useful to make the Member.. classes generic in the same way as ILinqKnowledgeBase - for
    /// validation, as well as potential manipulation power.
    /// </remarks>
    public class MemberPredicateSymbol
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstantSymbol"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        public MemberPredicateSymbol(MemberInfo memberInfo) => MemberInfo = memberInfo;

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> to which this symbol refers.
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <inheritdoc />
        public override string ToString() => MemberInfo.Name;

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is MemberPredicateSymbol otherMemberPredicateSymbol
                && MemberInfoEqualityComparer.Instance.Equals(MemberInfo, otherMemberPredicateSymbol.MemberInfo);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return MemberInfoEqualityComparer.Instance.GetHashCode(this.MemberInfo);
        }
    }
}
