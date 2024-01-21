// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration;

/// <summary>
/// Representation of the identifier of a <see cref="Constant"/> that refers to a particular element-valued
/// method or property or parameterless method call on a class representing the domain.
/// </summary>
/// <remarks>
/// Might ultimately be useful to make the Member..Identifier classes generic in the same way as ILinqKnowledgeBase - for
/// validation, as well as potential manipulation power.
/// </remarks>
public class MemberConstantIdentifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemberConstantIdentifier"/> class.
    /// </summary>
    /// <param name="memberInfo"></param>
    public MemberConstantIdentifier(MemberInfo memberInfo) => MemberInfo = memberInfo;

    /// <summary>
    /// Gets the <see cref="MemberInfo"/> to which this identifier refers.
    /// </summary>
    public MemberInfo MemberInfo { get; }

    /// <inheritdoc />
    public override string ToString() => MemberInfo.Name;

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is MemberConstantIdentifier otherMemberConstantIdentifier
            && MemberInfoEqualityComparer.Instance.Equals(MemberInfo, otherMemberConstantIdentifier.MemberInfo);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return MemberInfoEqualityComparer.Instance.GetHashCode(this.MemberInfo);
    }
}
