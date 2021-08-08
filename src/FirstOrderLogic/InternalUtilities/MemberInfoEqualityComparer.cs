using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.InternalUtilities
{
    /// <summary>
    /// Equality comparer for <see cref="MemberInfo"/>.
    /// </summary>
    /// <remarks>
    /// For reasons I haven't discovered but presumably exist (performance?), MemberInfo doesn't override
    /// equality (and MemberInfo instances aren't unified for the same member), so comparing two instances
    /// and expecting the same member to evaluate as equal doesn't work. Hence this class.
    /// </remarks>
    internal class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
    {
        public static MemberInfoEqualityComparer Instance { get; } = new MemberInfoEqualityComparer();

        public bool Equals(MemberInfo x, MemberInfo y) => x.Module == y.Module && x.MetadataToken == y.MetadataToken;

        public int GetHashCode(MemberInfo obj) => HashCode.Combine(obj.Module, obj.MetadataToken);
    }
}
