namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    public abstract class Constant : Term
    {
        /// <inheritdoc />
        public override sealed bool IsGroundTerm => true;
    }
}
