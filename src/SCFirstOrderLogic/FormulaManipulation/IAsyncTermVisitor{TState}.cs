// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Interface for asynchronous visitors of <see cref="Term"/> instances.
/// </para>
/// <para>
/// NB: This interface (in comparison to the non-generic <see cref="IAsyncTermVisitor"/>) is specifically for visitors that facilitate
/// state accumulation outside of the visitor instance itself.
/// </para>
/// </summary>
/// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
public interface IAsyncTermVisitor<in TState>
{
    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">The state for this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    Task VisitAsync(Function function, TState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">The state for this visitation.</param>
    /// <param name="cancellationToken">The cancellation token for the visitation.</param>
    Task VisitAsync(VariableReference variable, TState state, CancellationToken cancellationToken = default);
}
