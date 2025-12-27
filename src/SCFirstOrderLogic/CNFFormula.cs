// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaFormatting;
using SCFirstOrderLogic.FormulaManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic;

/// <summary>
/// <para>
/// Streamlined representation of a <see cref="Formula"/> in conjunctive normal form (CNF).
/// Consists of a set of <see cref="CNFClause"/>s.
/// </para>
/// <para>
/// Note that this type is NOT a subtype of <see cref="Formula"/>. To represent CNF as a <see cref="Formula"/>, 
/// <see cref="Conjunction"/>, <see cref="Disjunction"/>, <see cref="Predicate"/> and <see cref="Negation"/> 
/// should be used  as appropriate.
/// </para>
/// </summary>
public class CNFFormula : IEquatable<CNFFormula>
{
    private static readonly IEqualityComparer<HashSet<CNFClause>> ClausesEqualityComparer = HashSet<CNFClause>.CreateSetComparer();
    private readonly HashSet<CNFClause> clauses;

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFFormula"/> class from an enumerable of clauses.
    /// </summary>
    /// <param name="clauses">The set of clauses to be included in the formula.</param>
    // todo-bug?: allows creation of an empty formula
    public CNFFormula(IEnumerable<CNFClause> clauses)
        : this(new HashSet<CNFClause>(clauses))
    {
        if (Clauses.Count == 0)
        {
            throw new ArgumentException($"CNF formulas must have at least one clause.", nameof(clauses));
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFFormula"/> class from a <see cref="Formula"/> that is a conjunction of disjunctions of literals (a literal being a predicate or a negated predicate).
    /// </summary>
    /// <param name="cnfFormula">
    /// The formula, in CNF but represented as a <see cref="Formula"/>. An <see cref="ArgumentException"/> will be thrown if it is not a conjunction of disjunctions of literals.
    /// In other words, conversion to CNF is NOT carried out by this constructor.
    /// </param>
    public CNFFormula(Formula cnfFormula)
        : this(ConstructionVisitor.GetClauses(cnfFormula))
    {
    }

    // NB: We *could* actually use an immutable type to stop unscrupulous consumers from making it mutable by casting,
    // but this is a very low-level class, so I've opted to be lean and mean.
    internal CNFFormula(HashSet<CNFClause> clauses) => this.clauses = clauses;

    /// <summary>
    /// Gets the collection of clauses that comprise this CNF formula.
    /// </summary>
    public IReadOnlySet<CNFClause> Clauses => clauses;

    /// <summary>
    /// Converts this object to a <see cref="Formula"/>.
    /// </summary>
    /// <returns>A representation of this formula as a <see cref="Formula"/>.</returns>
    public Formula ToFormula()
    {
        Formula formula = Clauses.First().ToFormula();
        foreach (var clause in Clauses.Skip(1))
        {
            formula = new Conjunction(formula, clause.ToFormula());
        }

        return formula;
    }

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="FormulaFormatter"/> object and uses it to format the formula.
    /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
    /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
    /// of these things, instantiate your own <see cref="FormulaFormatter"/> instance.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => new FormulaFormatter().Format(this);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CNFFormula formula && Equals(formula);

    /// <inheritdoc />
    /// <remarks>
    /// Formulas that contain exactly the same collection of clauses are considered equal.
    /// </remarks>
    public bool Equals(CNFFormula? other)
    {
        return other != null && ClausesEqualityComparer.Equals(clauses, other.clauses);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Formulas that contain exactly the same collection of clauses are considered equal.
    /// </remarks>
    public override int GetHashCode()
    {
        return ClausesEqualityComparer.GetHashCode(clauses);
    }

    private class ConstructionVisitor : RecursiveFormulaVisitor
    {
        private readonly HashSet<CNFClause> clauses = new();

        public static HashSet<CNFClause> GetClauses(Formula formula)
        {
            var visitor = new ConstructionVisitor();
            visitor.Visit(formula);
            return visitor.clauses;
        }

        /// <inheritdoc />
        public override void Visit(Formula formula)
        {
            if (formula is Conjunction conjunction)
            {
                // The formula is already in CNF - so the root down until the individual clauses will all be Conjunctions - we just skip past those.
                Visit(conjunction);
            }
            else
            {
                // We've hit a clause.
                // Afterwards, we don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor that
                // we invoke here does so to figure out the details of the clause). So we can just return rather than invoking base.Visit.
                clauses.Add(new CNFClause(formula));
            }
        }
    }
}
