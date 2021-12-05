using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Reflection;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    public class Constant : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="valueExpr">An expression for obtaining the value of the constant.</param>
        public Constant(MemberInfo memberInfo) => Member = memberInfo;

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> instance for the logic behind this constant.
        /// </summary>
        public MemberInfo Member { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => true;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Constant otherConstant && MemberInfoEqualityComparer.Instance.Equals(otherConstant.Member, Member);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(MemberInfoEqualityComparer.Instance.GetHashCode(Member));
    }
}
