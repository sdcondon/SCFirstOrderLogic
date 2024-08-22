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
    /// Converts this sentence to conjunctive normal form.
    /// </summary>
    /// <returns>A new <see cref="CNFSentence"/> object.</returns>
    public static CNFSentence ToCNF(this Sentence sentence) => new(CNFSentenceConstructionVisitor.GetClauses(sentence));

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

    /// <summary>
    /// Sentence visitor that constructs a set of <see cref="CNFClause"/> objects from a <see cref="Sentence"/> in CNF.
    /// </summary>
    private class CNFSentenceConstructionVisitor : RecursiveSentenceVisitor
    {
        private readonly HashSet<CNFClause> clauses = new();

        public static HashSet<CNFClause> GetClauses(Sentence sentence)
        {
            var visitor = new CNFSentenceConstructionVisitor();
            visitor.Visit(CNFConversion.ApplyTo(sentence));
            return visitor.clauses;
        }

        /// <inheritdoc />
        public override void Visit(Sentence sentence)
        {
            if (sentence is Conjunction conjunction)
            {
                // The expression is already in CNF - so the root down until the individual clauses will all be Conjunctions - we just skip past those.
                Visit(conjunction);
            }
            else
            {
                // We've hit a clause.
                // Afterwards, we don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor that
                // we invoke here does so to figure out the details of the clause). So we can just return rather than invoking base.Visit.
                clauses.Add(new CNFClause(sentence));
            }
        }
    }
}