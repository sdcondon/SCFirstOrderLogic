namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Interface for visitors of <see cref="Term"/> instances.
    /// </summary>
    public interface ITermVisitor
    {
        /// <summary>
        /// Visits a <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        void Visit(Constant constant);

        /// <summary>
        /// Visits a <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        void Visit(Function function);

        /// <summary>
        /// Visits a <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="term">The variable to visit.</param>
        void Visit(VariableReference variable);
    }
}
