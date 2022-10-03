namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Interface for transformations of <see cref="Term"/> instances.
    /// <para/>
    /// NB: Essentially an interface for visitors with a return value.
    /// </summary>
    /// <typeparam name="TOut">The type that the transformation transforms the term to.</typeparam>
    public interface ITermTransformation<out TOut>
    {
        /// <summary>
        /// Applies the transformation to a <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="constant">The constant to transform.</param>
        TOut ApplyTo(Constant constant);

        /// <summary>
        /// Applies the transformation to a <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to transform.</param>
        TOut ApplyTo(Function function);

        /// <summary>
        /// Applies the transformation to a <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="variable">The variable reference to transform.</param>
        TOut ApplyTo(VariableReference variable);
    }
}
