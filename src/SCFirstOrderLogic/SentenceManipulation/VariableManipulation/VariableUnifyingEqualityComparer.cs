using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// <para>
/// Equality comparer for CNF clauses for which variable identifiers (though not their distinctness) are deemed irrelevant.
/// That is, an equality comparer that considers P(x, y) equal to P(a, b) (but of course distinct from P(x, x)).
/// </para>
/// <para>
/// NB: of course, such comparison is non-trivial in terms of performance. When an unambiguous ordering of 
/// literals can be established, consider transformation via <see cref="VariableManipulationExtensions.Ordinalise(Term)"/>
/// followed by equality comparison using plain old <see cref="object.Equals(object?)"/> instead.
/// </para>
/// </summary>
public class VariableUnifyingEqualityComparer : IEqualityComparer<CNFClause>
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
            return TryUpdateUnifier(x, y, new MutableVariableSubstitution());
        }
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] CNFClause obj)
    {
        return TransformForHashCode(obj).GetHashCode();
    }

    private static bool TryUpdateUnifier(CNFClause x, CNFClause y, MutableVariableSubstitution unifier)
    {
        if (x.Literals.Count != y.Literals.Count)
        {
            return false;
        }

        foreach (var literals in x.Literals.Zip(y.Literals, (x, y) => (x, y)))
        {
            if (!TryUpdateUnifier(literals.x, literals.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateUnifier(Literal x, Literal y, MutableVariableSubstitution unifier)
    {
        if (x.IsNegated != y.IsNegated)
        {
            return false;
        }

        return TryUpdateUnifier(x.Predicate, y.Predicate, unifier);
    }

    private static bool TryUpdateUnifier(Predicate x, Predicate y, MutableVariableSubstitution unifier)
    {
        if (!x.Identifier.Equals(y.Identifier)
            || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
        {
            if (!TryUpdateUnifier(args.x, args.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateUnifier(Term x, Term y, MutableVariableSubstitution unifier)
    {
        return (x, y) switch
        {
            (VariableReference variableX, VariableReference variableY) => TryUpdateUnifier(variableX, variableY, unifier),
            (Function functionX, Function functionY) => TryUpdateUnifier(functionX, functionY, unifier),
            _ => false
        };
    }

    private static bool TryUpdateUnifier(VariableReference x, VariableReference y, MutableVariableSubstitution unifier)
    {
        if (!unifier.Bindings.TryGetValue(x, out var boundXValue))
        {
            unifier.AddBinding(x, y);
        }
        else if (!boundXValue.Equals(y))
        {
            return false;
        }

        if (!unifier.Bindings.TryGetValue(y, out var boundYValue))
        {
            unifier.AddBinding(y, x);
        }
        else if (!boundYValue.Equals(x))
        {
            return false;
        }

        return true;
    }

    private static bool TryUpdateUnifier(Function x, Function y, MutableVariableSubstitution unifier)
    {
        if (!x.Identifier.Equals(y.Identifier) || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Arguments.Count; i++)
        {
            if (!TryUpdateUnifier(x.Arguments[i], y.Arguments[i], unifier))
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
