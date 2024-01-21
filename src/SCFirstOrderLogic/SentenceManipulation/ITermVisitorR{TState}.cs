// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Interface for visitors of <see cref="Term"/> instances.
/// </para>
/// <para>
/// NB: This interface (in comparison to the non-generic <see cref="ITermVisitor"/>) is specifically for visitors that facilitate
/// state accumulation outside of the visitor instance itself - state that is passed to the visitor by reference.
/// </para>
/// </summary>
/// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
public interface ITermVisitorR<TState>
{
    /// <summary>
    /// Visits a <see cref="Constant"/> instance.
    /// </summary>
    /// <param name="constant">The constant to visit.</param>
    /// <param name="state">A reference to the state for this visitation.</param>
    void Visit(Constant constant, ref TState state);

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">A reference to the state for this visitation.</param>
    void Visit(Function function, ref TState state);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">A reference to the state for this visitation.</param>
    void Visit(VariableReference variable, ref TState state);
}
