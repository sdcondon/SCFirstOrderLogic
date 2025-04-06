// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Interface for transformations of <see cref="Sentence"/> instances.
/// </para>
/// <para>
/// NB: Essentially an interface for visitors with a return value.
/// </para>
/// <para>
/// NB: This interface (in comparison to <see cref="ISentenceTransformation{TOut}"/>) is specifically
/// for transformations that facilitate state accumulation outside of the transformation instance itself.
/// </para>
/// </summary>
/// <typeparam name="TOut">The type that the transformation transforms the sentence to.</typeparam>
/// <typeparam name="TState">The type of state that this transformation works with.</typeparam>
public interface ISentenceTransformation<out TOut, in TState>
{
    /// <summary>
    /// Applies the transformation to a <see cref="Conjunction"/> instance.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(Conjunction conjunction, TState state);

    /// <summary>
    /// Applies the transformation to a <see cref="Disjunction"/> instance.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(Disjunction disjunction, TState state);

    /// <summary>
    /// Applies the transformation to a <see cref="Equivalence"/> instance. 
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(Equivalence equivalence, TState state);

    /// <summary>
    /// Applies the transformation to an <see cref="ExistentialQuantification"/> instance. 
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(ExistentialQuantification existentialQuantification, TState state);

    /// <summary>
    /// Applies the transformation to an <see cref="Implication"/> instance. 
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(Implication implication, TState state);

    /// <summary>
    /// Applies the transformation to a <see cref="Negation"/> instance. 
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(Negation negation, TState state);

    /// <summary>
    /// Applies the transformation to a <see cref="Predicate"/> instance. 
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(Predicate predicate, TState state);

    /// <summary>
    /// Applies the transformation to a <see cref="UniversalQuantification"/> instance. 
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to transform.</param>
    /// <param name="state">The state of this transformation.</param>
    TOut ApplyTo(UniversalQuantification universalQuantification, TState state);
}
