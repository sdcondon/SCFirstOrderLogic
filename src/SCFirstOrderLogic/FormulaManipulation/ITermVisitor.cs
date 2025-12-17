// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// Interface for visitors of <see cref="Term"/> instances.
/// </summary>
public interface ITermVisitor
{
    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    void Visit(Function function);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    void Visit(VariableReference variable);
}
