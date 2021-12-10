namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    /// <remarks>
    /// TODO: Should be called MemberConstant - Constant should be an abstract base class.
    /// </remarks>
    public abstract class Constant : Term
    {
        /// <inheritdoc />
        public override sealed bool IsGroundTerm => true;
    }
}
