using System.Collections.Generic;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. Specifically,
    /// represents a function that refers to a particular element-valued method or property of elements of
    /// the domain.
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: Might ultimately be useful to make the Member.. classes generic in the same way as KnowledgeBase - for
    /// validation, as well as potential manipulation power. OR simply delete this class as it adds no real value.
    /// </remarks>
    public class MemberFunction : Function
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFunction"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this function.</param>
        public MemberFunction(MemberInfo memberInfo, params Term[] arguments)
            : base(new MemberSymbol(memberInfo), arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFunction"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this function.</param>
        public MemberFunction(MemberInfo memberInfo, IList<Term> arguments)
            : base(new MemberSymbol(memberInfo), arguments)
        {
        }
    }
}
