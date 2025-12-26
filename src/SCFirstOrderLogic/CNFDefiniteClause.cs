// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic;

/// <summary>
/// Sub-type of <see cref="CNFClause"/> that adds methods and properties appropriate for definite clauses.
/// </summary>
public class CNFDefiniteClause : CNFClause
{
    /// <summary>
    /// Initialises a new instance of the <see cref="CNFDefiniteClause"/> class from an enumerable of literals.
    /// </summary>
    /// <param name="literals">The set of literals to be included in the clause. An <see cref="ArgumentException"/> will be thrown if there is not exactly one literal.</param>
    public CNFDefiniteClause(IEnumerable<Literal> literals)
        : base(new HashSet<Literal>(literals))
    {
        if (!IsDefiniteClause)
        {
            throw new ArgumentException($"Definite clauses have exactly one positive literal. Provided literals collection contains {Literals.Count(l => l.IsPositive)}.", nameof(literals));
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFDefiniteClause"/> class from a formula.
    /// </summary>
    /// <param name="cnfClause">The clause, represented as a <see cref="Formula"/>. An <see cref="ArgumentException"/> will be thrown if it is not a disjunction of literals (a literal being a predicate or a negated predicate).</param>
    public CNFDefiniteClause(Formula cnfClause)
        : base(cnfClause)
    {
        if (!IsDefiniteClause)
        {
            throw new ArgumentException($"Definite clauses have exactly one positive literal. Provided literals collection contains {Literals.Count(l => l.IsPositive)}.", nameof(cnfClause));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CNFDefiniteClause"/> class that is a copy of an existing <see cref="CNFClause"/>.
    /// </summary>
    /// <param name="definiteClause">The definite clause.</param>
    public CNFDefiniteClause(CNFClause definiteClause)
        : base(definiteClause.Literals)
    {
        if (!IsDefiniteClause)
        {
            throw new ArgumentException($"Provided clause must be a definite clause. {definiteClause} is not.", nameof(definiteClause));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CNFDefiniteClause"/> class that is just a unit clause.
    /// </summary>
    /// <param name="predicate">The sole predicate of the unit clause.</param>
    public CNFDefiniteClause(Predicate predicate)
        : base(predicate)
    {
    }

    /// <summary>
    /// Gets the consequent of this clause (that is, the Q in P₁ ∧ P₂ ∧ .. ∧ Pₙ ⇒ Q).
    /// </summary>
    public Predicate Consequent => Literals.Single(l => l.IsPositive).Predicate;

    /// <summary>
    /// Gets the conjuncts that combine to form the antecedent of this clause (that is, the P₁, .. Pₙ in P₁ ∧ P₂ ∧ .. ∧ Pₙ ⇒ Q).
    /// </summary>
    public IEnumerable<Predicate> Conjuncts => Literals.Where(l => l.IsNegated).Select(l => l.Predicate);
}
