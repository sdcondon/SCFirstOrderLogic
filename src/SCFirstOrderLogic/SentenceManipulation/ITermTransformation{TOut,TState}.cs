// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// <para>
    /// Interface for transformations of <see cref="Term"/> instances.
    /// </para>
    /// <para>
    /// NB: Essentially an interface for visitors with a return value.
    /// </para>
    /// <para>
    /// NB: This interface (in comparison to <see cref="ITermTransformation{TOut}"/>) is specifically
    /// for transformations that facilitate state accumulation outside of the transformation instance itself.
    /// </para>
    /// </summary>
    /// <typeparam name="TOut">The type that the transformation transforms the term to.</typeparam>
    /// <typeparam name="TState">The type of state that this transformation works with.</typeparam>
    public interface ITermTransformation<out TOut, in TState>
    {
        /// <summary>
        /// Applies the transformation to a <see cref="Constant"/> instance.
        /// </summary>
        /// <param name="constant">The constant to transform.</param>
        /// <param name="state">The state of this transformation.</param>
        TOut ApplyTo(Constant constant, TState state);

        /// <summary>
        /// Applies the transformation to a <see cref="Function"/> instance.
        /// </summary>
        /// <param name="function">The function to transform.</param>
        /// <param name="state">The state of this transformation.</param>
        TOut ApplyTo(Function function, TState state);

        /// <summary>
        /// Applies the transformation to a <see cref="VariableReference"/> instance.
        /// </summary>
        /// <param name="variable">The variable reference to transform.</param>
        /// <param name="state">The state of this transformation.</param>
        TOut ApplyTo(VariableReference variable, TState state);
    }
}
