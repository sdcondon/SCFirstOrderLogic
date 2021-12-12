using System.Collections.Generic;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>Predicate({term}, ..)</code>
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: Might ultimately be useful to make the Member.. classes generic in the same way as KnowledgeBase - for
    /// validation, as well as potential manipulation power. OR simply delete this class as it adds no real value.
    /// </remarks>
    public class MemberPredicate : Predicate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPredicate"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public MemberPredicate(MemberInfo memberInfo, params Term[] arguments)
            : base(new MemberSymbol(memberInfo), arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberPredicate"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public MemberPredicate(MemberInfo memberInfo, IList<Term> arguments)
            : base(new MemberSymbol(memberInfo), arguments)
        {
            // TODO-ROBUSTNESS: This is public - so should probably validate that its boolean valued and that the arguments match it..
        }
    }
}
