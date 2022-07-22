namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Interface for transformations of <see cref="Term"/> instances.
    /// <para/>
    /// NB: Essentially an interface for visitors with a return value. Nearly called it ITermQuery, but "query" already means something in FoL..
    /// </summary>
    /// <typeparam name="TOut">The type that the transformation transforms the sentence to.</typeparam>
    public interface ITermTransformation<out TOut>
    {
        /// <summary>
        /// Visits a <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="constant">The constant to visit.</param>
        TOut ApplyTo(Constant constant);

        /// <summary>
        /// Visits a <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        TOut ApplyTo(Function function);

        /// <summary>
        /// Visits a <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="variable">The variable reference to visit.</param>
        TOut ApplyTo(VariableReference variable);
    }
}
