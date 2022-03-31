using System.Collections.Generic;
using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic. Specifically,
    /// represents a predicate that refers to a particular boolean-valued method or property of elements of
    /// the domain (or the domain itself, in the case of ground predicates).
    /// </summary>
    /// <remarks>
    /// TODO-FUNCTIONALITY: Might ultimately be useful to make the Member.. classes generic in the same way as ILinqKnowledgeBase - for
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
