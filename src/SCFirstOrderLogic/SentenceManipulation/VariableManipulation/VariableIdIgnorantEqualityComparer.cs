// Copyright © 2023-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// <para>
/// Equality comparer for CNF clauses for which variable identifiers (though not their distinctness) are deemed irrelevant.
/// That is, an equality comparer that considers P(x, y) equal to P(a, b) - but distinct from P(x, x).
/// </para>
/// <para>
/// NB: of course, such comparison is costly in terms of performance. When an unambiguous ordering of literals
/// can be established, instead consider prior transformation via <see cref="VariableManipulationExtensions.Ordinalise(Literal)"/>,
/// followed by equality comparison using plain old <see cref="object.Equals(object?)"/>.
/// </para>
/// </summary>
// TODO-PERFORMANCE: The doc above does make it clear that this is a last resort, but I should defo take some time to try
// to optimise here. *Two* short-lived dictionaries in equality comparison? Could avoid one of 'em by effectively standardising
// apart as we go (e.g. making x vs y part of the key in a singular dict)? Also, the variable counts could easily be low enough
// that its not worth anything hash-based at all, and just a list is better. Benchmark me.
// TODO-BREAKING: VariableId*Agnostic*Comparer a better name? Cos its not really "ignorant" - its taking into account the fact that they could differ..
public class VariableIdIgnorantEqualityComparer : IEqualityComparer<CNFClause>, IEqualityComparer<Literal>, IEqualityComparer<Predicate>, IEqualityComparer<Term>
{
    private static readonly VariableReference VariableReferenceForHashCode = new(new {});

    /// <inheritdoc/>
    public bool Equals(CNFClause? x, CNFClause? y)
    {
        if (x == null)
        {
            return y == null;
        }
        else if (y == null)
        {
            return false;
        }
        else
        {
            return TryUpdateVariableMap(x, y, new(), new());
        }
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] CNFClause obj)
    {
        return TransformForHashCode(obj).GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(Literal? x, Literal? y)
    {
        if (x == null)
        {
            return y == null;
        }
        else if (y == null)
        {
            return false;
        }
        else
        {
            return TryUpdateVariableMap(x, y, new(), new());
        }
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Literal obj)
    {
        return TransformForHashCode(obj).GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(Predicate? x, Predicate? y)
    {
        if (x == null)
        {
            return y == null;
        }
        else if (y == null)
        {
            return false;
        }
        else
        {
            return TryUpdateVariableMap(x, y, new(), new());
        }
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Predicate obj)
    {
        return TransformForHashCode(obj).GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals(Term? x, Term? y)
    {
        if (x == null)
        {
            return y == null;
        }
        else if (y == null)
        {
            return false;
        }
        else
        {
            return TryUpdateVariableMap(x, y, new(), new());
        }
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] Term obj)
    {
        return TransformForHashCode(obj).GetHashCode();
    }

    private static bool TryUpdateVariableMap(
        CNFClause x,
        CNFClause y,
        Dictionary<VariableReference, VariableReference> xToY,
        Dictionary<VariableReference, VariableReference> yToX)
    {
        if (x.Literals.Count != y.Literals.Count)
        {
            return false;
        }

        foreach (var literals in x.Literals.Zip(y.Literals, (x, y) => (x, y)))
        {
            if (!TryUpdateVariableMap(literals.x, literals.y, xToY, yToX))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateVariableMap(
        Literal x,
        Literal y,
        Dictionary<VariableReference, VariableReference> xToY,
        Dictionary<VariableReference, VariableReference> yToX)
    {
        if (x.IsNegated != y.IsNegated)
        {
            return false;
        }

        return TryUpdateVariableMap(x.Predicate, y.Predicate, xToY, yToX);
    }

    private static bool TryUpdateVariableMap(
        Predicate x,
        Predicate y,
        Dictionary<VariableReference, VariableReference> xToY,
        Dictionary<VariableReference, VariableReference> yToX)
    {
        if (!x.Identifier.Equals(y.Identifier)
            || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
        {
            if (!TryUpdateVariableMap(args.x, args.y, xToY, yToX))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateVariableMap(
        Term x,
        Term y,
        Dictionary<VariableReference, VariableReference> xToY,
        Dictionary<VariableReference, VariableReference> yToX)
    {
        return (x, y) switch
        {
            (VariableReference variableX, VariableReference variableY) => TryUpdateVariableMap(variableX, variableY, xToY, yToX),
            (Function functionX, Function functionY) => TryUpdateVariableMap(functionX, functionY, xToY, yToX),
            _ => false
        };
    }

    private static bool TryUpdateVariableMap(
        VariableReference x,
        VariableReference y,
        Dictionary<VariableReference, VariableReference> xToY,
        Dictionary<VariableReference, VariableReference> yToX)
    {
        if (!xToY.TryGetValue(x, out var boundXValue))
        {
            xToY.Add(x, y);
        }
        else if (!boundXValue.Equals(y))
        {
            return false;
        }

        if (!yToX.TryGetValue(y, out var boundYValue))
        {
            yToX.Add(y, x);
        }
        else if (!boundYValue.Equals(x))
        {
            return false;
        }

        return true;
    }

    private static bool TryUpdateVariableMap(
        Function x,
        Function y,
        Dictionary<VariableReference, VariableReference> xToY,
        Dictionary<VariableReference, VariableReference> yToX)
    {
        if (!x.Identifier.Equals(y.Identifier) || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Arguments.Count; i++)
        {
            if (!TryUpdateVariableMap(x.Arguments[i], y.Arguments[i], xToY, yToX))
            {
                return false;
            }
        }

        return true;
    }

    private static CNFClause TransformForHashCode(CNFClause clause)
    {
        var isChanged = false;
        var transformed = new List<Literal>(clause.Literals.Count);

        foreach (var literal in clause.Literals)
        {
            var transformedLiteral = TransformForHashCode(literal);

            if (!ReferenceEquals(transformedLiteral, literal))
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new CNFClause(transformed);
        }

        return clause;
    }

    private static Literal TransformForHashCode(Literal literal)
    {
        var transformedPredicate = TransformForHashCode(literal.Predicate);

        if (ReferenceEquals(transformedPredicate, literal.Predicate))
        {
            return literal;
        }

        return new(transformedPredicate, literal.IsNegated);
    }

    private static Predicate TransformForHashCode(Predicate predicate)
    {
        var isChanged = false;
        var transformed = new Term[predicate.Arguments.Count];

        for (int i = 0; i < predicate.Arguments.Count; i++)
        {
            transformed[i] = TransformForHashCode(predicate.Arguments[i]);

            if (!ReferenceEquals(transformed[i], predicate.Arguments[i]))
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new Predicate(predicate.Identifier, transformed);
        }

        return predicate;
    }

    private static Term TransformForHashCode(Term term) => term switch
    {
        VariableReference => VariableReferenceForHashCode,
        Function function => TransformForHashCode(function),
        _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'", nameof(term))
    };

    private static Function TransformForHashCode(Function function)
    {
        var isChanged = false;
        var transformed = new Term[function.Arguments.Count];

        for (int i = 0; i < function.Arguments.Count; i++)
        {
            transformed[i] = TransformForHashCode(function.Arguments[i]);

            if (!ReferenceEquals(transformed[i], function.Arguments[i]))
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new Function(function.Identifier, transformed);
        }

        return function;
    }
}
