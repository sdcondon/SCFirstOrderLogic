// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.Normalisation;

/// <summary>
/// Extension methods related to normalisation.
/// </summary>
public static class NormalisationExtensions
{
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
            if (!newIdentifiersByOld!.TryGetValue(oldIdentifier, out var newIdentifier))
            {
                newIdentifier = newIdentifiersByOld[oldIdentifier] = new StandardisedVariableIdentifier(oldIdentifier.OriginalVariableScope, oldIdentifier.OriginalSentence);
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

#if false
// TODO: awkwardness - don't like the need for that explanatory comment about the type
// of the new identifiers. Perhaps look at adding this at the same time as moving/renaming
// the normalisation identifiers, to make it clear that they are only one way of representing
// the concepts, and are specific to our conversion classes.

    /// <summary>
    /// <para>
    /// Constructs and returns a literal that is the same as this one, except for the
    /// fact that all referenced variable identifiers are replaced with new ones
    /// (even if they are already standardised).
    /// </para>
    /// <para>
    /// NB: The new identifiers are just <see cref="object"/>s, not <see cref="StandardisedVariableIdentifier"/>s.
    /// The assumption is that this functionality will be used in fairly low-level computation
    /// and all we want is uniqueness, not the extra props that <see cref="StandardisedVariableIdentifier"/>
    /// provides - props that would be difficult to populate sensibly anyway.
    /// </para>
    /// </summary>
    /// <returns>
    /// The new literal.
    /// </returns>
    public static Literal Standardise(this Literal literal)
    {
        return Standardise(literal, new Dictionary<object, object>());
    }

    /// <summary>
    /// <para>
    /// Constructs and returns a predicate that is the same as this one, except for the
    /// fact that all referenced variable identifiers are replaced with new ones
    /// (even if they are already standardised).
    /// </para>
    /// <para>
    /// NB: The new identifiers are just <see cref="object"/>s, not <see cref="StandardisedVariableIdentifier"/>s.
    /// The assumption is that this functionality will be used in fairly low-level computation
    /// and all we want is uniqueness, not the extra props that <see cref="StandardisedVariableIdentifier"/>
    /// provides - props that would be difficult to populate sensibly anyway.
    /// </para>
    /// </summary>
    /// <returns>
    /// The new predicate.
    /// </returns>
    public static Predicate Standardise(this Predicate predicate)
    {
        return Standardise(predicate, new Dictionary<object, object>());
    }

    /// <summary>
    /// <para>
    /// Constructs and returns a term that is the same as this one, except for the
    /// fact that all referenced variable identifiers are replaced with new ones
    /// (even if they are already standardised).
    /// </para>
    /// <para>
    /// NB: The new identifiers are just <see cref="object"/>s, not <see cref="StandardisedVariableIdentifier"/>s.
    /// The assumption is that this functionality will be used in fairly low-level computation
    /// and all we want is uniqueness, not the extra props that <see cref="StandardisedVariableIdentifier"/>
    /// provides - props that would be difficult to populate sensibly anyway.
    /// </para>
    /// </summary>
    /// <returns>
    /// The new term.
    /// </returns>
    public static Term Standardise(this Term term)
    {
        return Standardise(term, new Dictionary<object, object>());
    }

    private static Literal Standardise(Literal literal, Dictionary<object, object> idMap)
    {
        return new(Standardise(literal.Predicate, idMap), literal.IsNegated);
    }

    private static Predicate Standardise(Predicate predicate, Dictionary<object, object> idMap)
    {
        return new(predicate.Identifier, predicate.Arguments.Select(t => Standardise(t, idMap)).ToArray());
    }

    private static Term Standardise(Term term, Dictionary<object, object> idMap)
    {
        return term switch
        {
            VariableReference v => new VariableReference(GetOrAddNewIdentifier(v.Identifier)),
            Function f => new Function(f.Identifier, f.Arguments.Select(t => Standardise(t, idMap)).ToArray()),
            _ => throw new ArgumentException($"Unexpected term type '{term.GetType()}' encountered", nameof(term)),
        };

        object GetOrAddNewIdentifier(object oldIdentifier)
        {
            if (!idMap.TryGetValue(oldIdentifier, out var newIdentifier))
            {
                newIdentifier = idMap[oldIdentifier] = new();
            }

            return newIdentifier;
        }
    }
#endif
}