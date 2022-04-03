using System.Reflection;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic. Specifically,
    /// represents a constant that refers to a particular element-valued method or property or parameterless method
    /// call on a class representing the domain.
    /// </summary>
    /// <remarks>
    /// TODO-ZZZ/MINOR-FUNCTIONALITY: Might ultimately be useful to make the Member.. classes generic in the same way as ILinqKnowledgeBase - for
    /// validation, as well as potential manipulation power. OR simply delete this class as it adds no real value.
    /// </remarks>
    public class MemberConstant : Constant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="memberInfo">An expression for obtaining the value of the constant.</param>
        public MemberConstant(MemberInfo memberInfo)
            : base(new MemberSymbol(memberInfo))
        {
        }
    }
}
