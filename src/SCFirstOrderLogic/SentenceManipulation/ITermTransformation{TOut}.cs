// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Interface for transformations of <see cref="Term"/> instances.
/// </para>
/// <para>
/// NB: Essentially an interface for visitors with a return value.
/// </para>
/// </summary>
/// <typeparam name="TOut">The type that the transformation transforms the term to.</typeparam>
public interface ITermTransformation<out TOut>
{
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
