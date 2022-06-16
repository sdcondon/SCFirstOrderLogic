﻿namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// Interface for visitors of <see cref="Term"/> instances.
    /// <para/>
    /// NB: This interface (in comparison to the non-generic <see cref="ITermVisitor"/>) is specifically for visitors that facilitate
    /// state accumulation outside of the visitor instance itself.
    /// </summary>
    /// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
    public interface ITermVisitor<TState>
    {
        /// <summary>
        /// Visits a <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="term">The constant to visit.</param>
        void Visit(Constant constant, ref TState state);

        /// <summary>
        /// Visits a <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to visit.</param>
        void Visit(Function function, ref TState state);

        /// <summary>
        /// Visits a <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="term">The variable reference to visit.</param>
        void Visit(VariableReference variable, ref TState state);
    }
}
