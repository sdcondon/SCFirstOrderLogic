// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading.Tasks;

namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// Interface for asynchronous visitors of <see cref="Formula"/> instances.
/// </summary>
public interface IAsyncFormulaVisitor
{
    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to visit.</param>
    Task VisitAsync(Conjunction conjunction);

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    Task VisitAsync(Disjunction disjunction);

    /// <summary>
    /// Visits a <see cref="Equivalence"/> instance. 
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    Task VisitAsync(Equivalence equivalence);

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    Task VisitAsync(ExistentialQuantification existentialQuantification);

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    Task VisitAsync(Implication implication);

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    Task VisitAsync(Negation negation);

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    Task VisitAsync(Predicate predicate);

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    Task VisitAsync(UniversalQuantification universalQuantification);
}
