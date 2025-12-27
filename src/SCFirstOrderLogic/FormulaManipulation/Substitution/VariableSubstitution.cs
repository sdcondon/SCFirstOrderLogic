// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCFirstOrderLogic.FormulaManipulation.Substitution;

/// <summary>
/// Formula transformation class that makes some substitutions for <see cref="VariableReference"/> instances.
/// </summary>
public class VariableSubstitution : RecursiveFormulaTransformation
{
    /// <summary>
    /// The individual substitutions made by this substitution.
    /// </summary>
    protected readonly Dictionary<VariableReference, Term> bindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableSubstitution"/> class that is empty.
    /// </summary>
    public VariableSubstitution()
        : this(Enumerable.Empty<KeyValuePair<VariableReference, Term>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableSubstitution"/> class that uses a given set of bindings.
    /// </summary>
    /// <param name="bindings">The bindings to use.</param>
    public VariableSubstitution(IEnumerable<KeyValuePair<VariableReference, Term>> bindings)
    {
        this.bindings = new(bindings);
        Bindings = new ReadOnlyDictionary<VariableReference, Term>(this.bindings);
    }

    /// <summary>
    /// Gets the individual bindings of this substitution -
    /// a mapping from each replaced variable reference to the term that replaces it.
    /// </summary>
    public IReadOnlyDictionary<VariableReference, Term> Bindings { get; }

    /// <summary>
    /// Creates a read-only copy of this substitution.
    /// </summary>
    /// <returns>A new <see cref="VariableSubstitution"/> instance with the same bindings as this one.</returns>
    public VariableSubstitution CopyAsReadOnly() => new(bindings);

    /// <summary>
    /// Creates a read-only copy of this substitution, with an additional binding.
    /// </summary>
    /// <param name="additionalBinding">The binding to add to the clone.</param>
    /// <returns>A new <see cref="VariableSubstitution"/> instance with the same bindings as this one, plus the given additional one.</returns>
    public VariableSubstitution CopyAndAdd(KeyValuePair<VariableReference, Term> additionalBinding) => new(bindings.Append(additionalBinding));

    /// <summary>
    /// Creates a read-only copy of this substitution, with some additional bindings.
    /// </summary>
    /// <param name="additionalBindings">The bindings to add to the clone.</param>
    /// <returns>A new <see cref="VariableSubstitution"/> instance with the same bindings as this one, plus the given additional ones.</returns>
    public VariableSubstitution CopyAndAdd(IEnumerable<KeyValuePair<VariableReference, Term>> additionalBindings) => new(bindings.Concat(additionalBindings));

    /// <summary>
    /// Creates a mutable copy of this substitution.
    /// </summary>
    /// <returns>A new <see cref="MutableVariableSubstitution"/> instance with the same bindings as this one.</returns>
    public MutableVariableSubstitution CopyAsMutable() => new(bindings);

    /// <summary>
    /// Applies this substitution to a <see cref="Conjunction"/> instance.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Conjunction"/>.</returns>
    public override Conjunction ApplyTo(Conjunction conjunction)
    {
        return (Conjunction)base.ApplyTo(conjunction);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="Disjunction"/> instance.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Disjunction"/>.</returns>
    public override Disjunction ApplyTo(Disjunction disjunction)
    {
        return (Disjunction)base.ApplyTo(disjunction);
    }

    /// <summary>
    /// Applies this substitution to an <see cref="Equivalence"/> instance. 
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Equivalence"/>.</returns>
    public override Equivalence ApplyTo(Equivalence equivalence)
    {
        return (Equivalence)base.ApplyTo(equivalence);
    }

    /// <summary>
    /// Applies this substitution to an <see cref="ExistentialQuantification"/> instance. 
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to transform.</param>
    /// <returns>The transformed <see cref="ExistentialQuantification"/>.</returns>
    public override ExistentialQuantification ApplyTo(ExistentialQuantification existentialQuantification)
    {
        return (ExistentialQuantification)base.ApplyTo(existentialQuantification);
    }

    /// <summary>
    /// Applies this substitution to an <see cref="Implication"/> instance. 
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Implication"/>.</returns>
    public override Implication ApplyTo(Implication implication)
    {
        return (Implication)base.ApplyTo(implication);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="Predicate"/> instance. 
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Predicate"/>.</returns>
    public override Predicate ApplyTo(Predicate predicate)
    {
        return (Predicate)base.ApplyTo(predicate);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="Negation"/> instance. 
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Negation"/>.</returns>
    public override Negation ApplyTo(Negation negation)
    {
        return (Negation)base.ApplyTo(negation);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="Quantification"/> instance. 
    /// </summary>
    /// <param name="quantification">The <see cref="Quantification"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Quantification"/>.</returns>
    public override Quantification ApplyTo(Quantification quantification)
    {
        return (Quantification)base.ApplyTo(quantification);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="UniversalQuantification"/> instance. 
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to transform.</param>
    /// <returns>The transformed <see cref="UniversalQuantification"/>.</returns>
    public override UniversalQuantification ApplyTo(UniversalQuantification universalQuantification)
    {
        return (UniversalQuantification)base.ApplyTo(universalQuantification);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="VariableReference"/> instance.
    /// </summary>
    /// <param name="variable">The variable to transform.</param>
    /// <returns>The transformed <see cref="Term"/>.</returns>
    public override Term ApplyTo(VariableReference variable)
    {
        if (bindings.TryGetValue(variable, out var substitutedTerm))
        {
            // We need to call base.ApplyTo because we might be switching in a term
            // that itself is or contains variables that also need substituting.
            // TODO-ROBUSTNESS: In theory makes it possible to get us stuck in a loop,
            // but (doesn't happen as a result of anything internal with an occurs check, and)
            // don't want to add the performance burden of checking for this every time.
            // Could add a separate validation method to allow consumers to opt-in to it?
            return base.ApplyTo(substitutedTerm);
        }

        return variable;
    }

    /// <summary>
    /// Applies this substitution to a <see cref="Function"/> instance.
    /// </summary>
    /// <param name="function">The function to transform.</param>
    /// <returns>The transformed <see cref="Function"/>.</returns>
    public override Function ApplyTo(Function function)
    {
        return (Function)base.ApplyTo(function);
    }

    /// <summary>
    /// Applies this substitution to a <see cref="CNFFormula"/> instance.
    /// </summary>
    /// <param name="cnfFormula">The <see cref="CNFFormula"/> instance to transform.</param>
    /// <returns>The transformed <see cref="CNFFormula"/>.</returns>
    public CNFFormula ApplyTo(CNFFormula cnfFormula)
    {
        var isChanged = false;
        var transformedClauses = new HashSet<CNFClause>(cnfFormula.Clauses.Count);

        foreach (var clause in cnfFormula.Clauses)
        {
            var transformedClause = ApplyTo(clause);
            transformedClauses.Add(transformedClause);

            if (transformedClause != clause)
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new CNFFormula(transformedClauses);
        }

        return cnfFormula;
    }

    /// <summary>
    /// Applies this substitution to a <see cref="CNFClause"/> instance.
    /// </summary>
    /// <param name="cnfClause">The <see cref="CNFClause"/> instance to transform.</param>
    /// <returns>The transformed <see cref="CNFClause"/>.</returns>
    public CNFClause ApplyTo(CNFClause cnfClause)
    {
        var isChanged = false;
        var transformedLiterals = new HashSet<Literal>(cnfClause.Literals.Count);

        foreach (var literal in cnfClause.Literals)
        {
            var transformedLiteral = ApplyTo(literal);
            transformedLiterals.Add(transformedLiteral);

            if (transformedLiteral != literal)
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new CNFClause(transformedLiterals);
        }

        return cnfClause;
    }

    /// <summary>
    /// Applies this substitution to a <see cref="Literal"/> instance.
    /// </summary>
    /// <param name="literal">The <see cref="Literal"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Literal"/>.</returns>
    public Literal ApplyTo(Literal literal)
    {
        var transformedPredicate = ApplyTo(literal.Predicate);

        if (transformedPredicate != literal.Predicate)
        {
            return new Literal(transformedPredicate, literal.IsNegative);
        }

        return literal;
    }

    /// <inheritdoc />
    // TODO-BUG-ARGUABLE: no normalisation - {X/Y, Y/C} and {X/C, Y/C} give the same result when applied but are not viewed as the same
    public override bool Equals(object? obj)
    {
        if (obj is not VariableSubstitution otherSubstitution)
        {
            return false;
        }

        foreach (var kvp in bindings)
        {
            if (!otherSubstitution.bindings.TryGetValue(kvp.Key, out var otherValue) || !kvp.Value.Equals(otherValue))
            {
                return false;
            }
        }

        foreach (var otherKvp in otherSubstitution.bindings)
        {
            if (!bindings.TryGetValue(otherKvp.Key, out var value) || !otherKvp.Value.Equals(value))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hashCode = new();

        foreach (var kvp in bindings)
        {
            hashCode.Add(kvp.Key);
            hashCode.Add(kvp.Value);
        }

        return hashCode.ToHashCode();
    }
}
