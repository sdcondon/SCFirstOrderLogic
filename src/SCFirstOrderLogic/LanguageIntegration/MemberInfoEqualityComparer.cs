using System;
using System.Collections.Generic;
using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Equality comparer for <see cref="MemberInfo"/>.
    /// <para/>
    /// For reasons I haven't discovered but are presumably good (performance?), MemberInfo doesn't override
    /// equality (and MemberInfo instances aren't unified for the same member), so comparing two instances
    /// and expecting the same member to evaluate as equal doesn't work. Hence this class.
    /// </summary>
    internal class MemberInfoEqualityComparer : IEqualityComparer<MemberInfo>
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="MemberInfoEqualityComparer"/> class.
        /// </summary>
        public static MemberInfoEqualityComparer Instance { get; } = new MemberInfoEqualityComparer();

        /// <inheritdoc />
        public bool Equals(MemberInfo x, MemberInfo y) => x.Module == y.Module && x.MetadataToken == y.MetadataToken;

        /// <inheritdoc />
        public int GetHashCode(MemberInfo obj) => HashCode.Combine(obj.Module, obj.MetadataToken);
    }
}
