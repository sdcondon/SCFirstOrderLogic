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
/// Streamlined representation of an individual clause (i.e. a disjunction of <see cref="Literal"/>s) of a first-order logic formula in conjunctive normal form.
/// Consists of a set of <see cref="Literal"/>s.
/// </para>
/// <para>
/// Note that this type is NOT a subtype of <see cref="Formula"/>. To represent a clause as a <see cref="Formula"/>, 
/// <see cref="Disjunction"/>, <see cref="Predicate"/> and <see cref="Negation"/> should be used as appropriate.
/// </para>
/// </summary>
public class CNFClause : IEquatable<CNFClause>
{
    private static readonly IEqualityComparer<HashSet<Literal>> LiteralsEqualityComparer = HashSet<Literal>.CreateSetComparer();
    private readonly HashSet<Literal> literals;

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFClause"/> class from an enumerable of literals.
    /// </summary>
    /// <param name="literals">The set of literals to be included in the clause.</param>
    public CNFClause(IEnumerable<Literal> literals)
        : this(new HashSet<Literal>(literals))
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFClause"/> class from a formula that is a disjunction of literals (a literal being a predicate or a negated predicate).
    /// </summary>
    /// <param name="cnfClause">The clause, represented as a <see cref="Formula"/>. An <see cref="ArgumentException"/> will be thrown if it is not a disjunction of literals.</param>
    public CNFClause(Formula cnfClause)
        : this(ConstructionVisitor.GetLiterals(cnfClause))
    {
    }

    // TODO-ZZ-ROBUSTNESS: We *could* actually use an immutable type to stop unscrupulous consumers from making it mutable by casting,
    // but this is a very low-level class, so I've opted to be lean and mean. Yes, this does mean that its possible for
    // consumers to break the empty clause (below), so.. yeah, maybe I should fix it..
    internal CNFClause(HashSet<Literal> literals) => this.literals = literals;

    /// <summary>
    /// Gets an instance of the empty clause.
    /// </summary>
    public static CNFClause Empty { get; } = new CNFClause(Array.Empty<Literal>());

    /// <summary>
    /// Gets the collection of literals that comprise this clause.
    /// </summary>
    public IReadOnlySet<Literal> Literals => literals;

    /// <summary>
    /// Gets a value indicating whether this is a Horn clause - that is, whether at most one of its literals is positive.
    /// </summary>
    public bool IsHornClause
    {
        get
        {
            var i = 0;
            using var enumerator = Literals.GetEnumerator();

            while (i < 2 && enumerator.MoveNext())
            {
                if (enumerator.Current.IsPositive)
                {
                    i++;
                }
            }

            return i < 2;
        }
    }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether this is a definite clause - that is, whether exactly one of its literals is positive.
    /// </para>
    /// <para>
    /// NB: this means that the clause can be written in the form L₁ ∧ L₂ ∧ .. ∧ Lₙ ⇒ L (where none of the literals is negated)
    /// - or is simply a single non-negated literal.
    /// </para>
    /// </summary>
    public bool IsDefiniteClause => Literals.Count(l => l.IsPositive) == 1;

    /// <summary>
    /// Gets a value indicating whether this is a goal clause - that is, whether none of its literals is positive.
    /// </summary>
    public bool IsGoalClause => !Literals.Any(l => l.IsPositive);

    /// <summary>
    /// Gets a value indicating whether this is a unit clause - that is, whether it contains exactly one literal.
    /// </summary>
    public bool IsUnitClause => Literals.Count == 1;

    /// <summary>
    /// Gets a value indicating whether this is an empty clause (that by convention evaluates to false). Can occur as a result of resolution.
    /// </summary>
    public bool IsEmpty => Literals.Count == 0;

    /// <summary>
    /// Converts the clause to a <see cref="Formula"/>
    /// </summary>
    /// <returns>A representation of this clause as a <see cref="Formula"/>.</returns>
    public Formula ToFormula()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Cannot convert empty clause to a formula.");
        }

        Formula formula = Literals.First().ToFormula();
        foreach (var literal in Literals.Skip(1))
        {
            formula = new Disjunction(formula, literal.ToFormula());
        }

        return formula;
    }

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="FormulaFormatter"/> object and uses it to format the clause.
    /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
    /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
    /// of these things, instantiate your own <see cref="FormulaFormatter"/> instance.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => new FormulaFormatter().Format(this);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CNFClause clause && Equals(clause);

    /// <inheritdoc />
    /// <remarks>
    /// Clauses that contain exactly the same collection of literals are considered equal.
    /// </remarks>
    public bool Equals(CNFClause? other)
    {
        return other != null && LiteralsEqualityComparer.Equals(literals, other.literals);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Clauses that contain exactly the same collection of literals are considered equal.
    /// </remarks>
    public override int GetHashCode()
    {
        return LiteralsEqualityComparer.GetHashCode(literals);
    }

    private class ConstructionVisitor : RecursiveFormulaVisitor
    {
        private readonly HashSet<Literal> literals = new();

        public static HashSet<Literal> GetLiterals(Formula formula)
        {
            var visitor = new ConstructionVisitor();
            visitor.Visit(formula);
            return visitor.literals;
        }

        public override void Visit(Formula formula)
        {
            if (formula is Disjunction disjunction)
            {
                // The formula is assumed to be a clause (i.e. a disjunction of literals) - so just skip past all the disjunctions at the root.
                base.Visit(disjunction);
            }
            else
            {
                // Assume we've hit a literal. NB will throw if its not actually a literal.
                // Afterwards, we don't need to look any further down the tree for the purposes of this class (though the Literal ctor that
                // we invoke here does so to figure out the details of the literal). So we can just return rather than invoking base.Visit.
                literals.Add(new Literal(formula));
            }
        }
    }
}
