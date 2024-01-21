// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Interface for transformations of <see cref="Sentence"/> instances.
/// </para>
/// <para>
/// NB: Essentially an interface for visitors with a return value.
/// </para>
/// </summary>
/// <typeparam name="TOut">The type that the transformation transforms the sentence to.</typeparam>
public interface ISentenceTransformation<out TOut>
{
    /// <summary>
    /// Applies the transformation to a <see cref="Conjunction"/> instance.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to transform.</param>
    TOut ApplyTo(Conjunction conjunction);

    /// <summary>
    /// Applies the transformation to a <see cref="Disjunction"/> instance.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to transform.</param>
    TOut ApplyTo(Disjunction disjunction);

    /// <summary>
    /// Applies the transformation to a <see cref="Equivalence"/> instance. 
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to transform.</param>
    TOut ApplyTo(Equivalence equivalence);

    /// <summary>
    /// Applies the transformation to an <see cref="ExistentialQuantification"/> instance. 
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to transform.</param>
    TOut ApplyTo(ExistentialQuantification existentialQuantification);

    /// <summary>
    /// Applies the transformation to an <see cref="Implication"/> instance. 
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to transform.</param>
    TOut ApplyTo(Implication implication);

    /// <summary>
    /// Applies the transformation to a <see cref="Negation"/> instance. 
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to transform.</param>
    TOut ApplyTo(Negation negation);

    /// <summary>
    /// Applies the transformation to a <see cref="Predicate"/> instance. 
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to transform.</param>
    TOut ApplyTo(Predicate predicate);

    /// <summary>
    /// Applies the transformation to a <see cref="UniversalQuantification"/> instance. 
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to transform.</param>
    TOut ApplyTo(UniversalQuantification universalQuantification);
}
