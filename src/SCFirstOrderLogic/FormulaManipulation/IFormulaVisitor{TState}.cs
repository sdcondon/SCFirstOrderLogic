// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// <para>
/// Interface for visitors of <see cref="Formula"/> instances.
/// </para>
/// <para>
/// NB: This interface (in comparison to the non-generic <see cref="IFormulaVisitor"/>) is specifically for visitors that facilitate
/// state accumulation outside of the visitor instance itself.
/// </para>
/// </summary>
/// <typeparam name="TState">The type of state that this visitor works with.</typeparam>
public interface IFormulaVisitor<in TState>
{
    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(Conjunction conjunction, TState state);

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(Disjunction disjunction, TState state);

    /// <summary>
    /// Visits a <see cref="Equivalence"/> instance. 
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(Equivalence equivalence, TState state);

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(ExistentialQuantification existentialQuantification, TState state);

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(Implication implication, TState state);

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(Negation negation, TState state);

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(Predicate predicate, TState state);

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    void Visit(UniversalQuantification universalQuantification, TState state);
}
