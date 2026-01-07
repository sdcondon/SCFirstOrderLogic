// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Interface for visitors of <see cref="Term"/> instances.
/// </para>
/// <para>
/// NB: This interface (in comparison to the non-generic <see cref="ITermVisitor"/>) is specifically for visitors that facilitate
/// state accumulation outside of the visitor instance itself.
/// </para>
/// </summary>
/// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
public interface ITermVisitor<in TState>
{
    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">The state for this visitation.</param>
    void Visit(Function function, TState state);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">The state for this visitation.</param>
    void Visit(VariableReference variable, TState state);
}
