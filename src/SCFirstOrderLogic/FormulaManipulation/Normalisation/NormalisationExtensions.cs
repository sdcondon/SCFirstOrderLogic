// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

/// <summary>
/// Extension methods related to normalisation.
/// </summary>
public static class NormalisationExtensions
{
    /// <summary>
    /// Converts this formula to conjunctive normal form.
    /// </summary>
    /// <returns>A new <see cref="CNFFormula"/> object.</returns>
    public static CNFFormula ToCNF(this Formula formula) => new(CNFConversion.ApplyTo(formula));

    /// <summary>
    /// Constructs and returns a clause that is the same as this one, except for the
    /// fact that all referenced (standardised) variable declarations are replaced with new ones.
    /// </summary>
    /// <returns>
    /// A clause that is the same as this one, except for the fact that all referenced
    /// variables are replaced with new ones.
    /// </returns>
    public static CNFClause Restandardise(this CNFClause clause)
    {
        var newIdentifiersByOld = new Dictionary<StandardisedVariableIdentifier, StandardisedVariableIdentifier>();
        return new CNFClause(clause.Literals.Select(RestandardiseLiteral));

        Literal RestandardiseLiteral(Literal literal) => new(RestandardisePredicate(literal.Predicate), literal.IsNegated);

        Predicate RestandardisePredicate(Predicate predicate) => new(predicate.Identifier, predicate.Arguments.Select(RestandardiseTerm).ToArray());

        Term RestandardiseTerm(Term term) => term switch
        {
            VariableReference v => new VariableReference(GetOrAddNewIdentifier((StandardisedVariableIdentifier)v.Identifier)),
            Function f => new Function(f.Identifier, f.Arguments.Select(RestandardiseTerm).ToArray()),
            _ => throw new ArgumentException($"Unexpected term type '{term.GetType()}' encountered", nameof(term)),
        };

        StandardisedVariableIdentifier GetOrAddNewIdentifier(StandardisedVariableIdentifier oldIdentifier)
        {
            if (!newIdentifiersByOld.TryGetValue(oldIdentifier, out var newIdentifier))
            {
                newIdentifier = newIdentifiersByOld[oldIdentifier] = new StandardisedVariableIdentifier(oldIdentifier.OriginalVariableScope, oldIdentifier.OriginalFormula);
            }

            return newIdentifier;
        }
    }

    /// <summary>
    /// Constructs and returns a clause that is the same as this one, except for the
    /// fact that all variable declarations are replaced with new ones.
    /// </summary>
    /// <returns>
    /// A clause that is the same as this one, except for the fact that all variable
    /// references are replaced with new ones.
    /// </returns>
    public static CNFDefiniteClause Restandardise(this CNFDefiniteClause clause)
    {
        return new(Restandardise((CNFClause)clause));
    }
}