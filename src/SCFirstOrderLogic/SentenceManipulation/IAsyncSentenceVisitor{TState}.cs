// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading.Tasks;

namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Interface for asynchronous visitors of <see cref="Sentence"/> instances.
/// </para>
/// <para>
/// NB: This interface (in comparison to the non-generic <see cref="IAsyncSentenceVisitor"/>) is specifically for visitors that facilitate
/// state accumulation outside of the visitor instance itself.
/// </para>
/// </summary>
/// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
public interface IAsyncSentenceVisitor<in TState>
{
    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(Conjunction conjunction, TState state);

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(Disjunction disjunction, TState state);

    /// <summary>
    /// Visits a <see cref="Equivalence"/> instance. 
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(Equivalence equivalence, TState state);

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(ExistentialQuantification existentialQuantification, TState state);

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(Implication implication, TState state);

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(Negation negation, TState state);

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(Predicate predicate, TState state);

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    Task VisitAsync(UniversalQuantification universalQuantification, TState state);
}
