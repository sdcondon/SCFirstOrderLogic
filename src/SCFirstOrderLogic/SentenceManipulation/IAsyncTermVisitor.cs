// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading.Tasks;

namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// Interface for asynchronous visitors of <see cref="Term"/> instances.
/// </summary>
public interface IAsyncTermVisitor
{
    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    Task VisitAsync(Function function);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    Task VisitAsync(VariableReference variable);
}
