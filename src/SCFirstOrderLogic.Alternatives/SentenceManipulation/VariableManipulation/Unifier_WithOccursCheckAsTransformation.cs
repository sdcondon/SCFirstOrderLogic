// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Diagnostics.CodeAnalysis;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// <para>
/// Utility class for creating unifiers for literals.
/// See §9.2.2 ("Unification") of 'Artificial Intelligence: A Modern Approach' for an explanation of this algorithm.
/// </para>
/// <para>
/// Differs from production version by using an occurs check that is a SentenceTransformation.
/// </para>
/// </summary>
public static class Unifier_WithOccursCheckAsTransformation
{
    /// <summary>
    /// Attempts to create the most general unifier for two literals.
    /// </summary>
    /// <param name="x">One of the two literals to attempt to create a unifier for.</param>
    /// <param name="y">One of the two literals to attempt to create a unifier for.</param>
    /// <param name="unifier">If the literals can be unified, this out parameter will be the unifier (which can then be applied with <see cref="ApplyTo"/>).</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryCreate(Literal x, Literal y, [NotNullWhen(returnValue: true)] out VariableSubstitution? unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (!TryUnify(x, y, unifierAttempt))
        {
            unifier = null;
            return false;
        }

        unifier = unifierAttempt.CopyAsReadOnly();
        return true;
    }

    private static bool TryUnify(Literal x, Literal y, MutableVariableSubstitution unifier)
    {
        if (x.IsNegated != y.IsNegated || !x.Predicate.Identifier.Equals(y.Predicate.Identifier))
        {
            return false;
        }

        // WOULD-BE-A-BUG-IF-THIS-WERE-PROD-CODE?: Makes the assumption that same identifier means same number of arguments.
        // It is possible to confuse this algorithm by passing literals where that isn't true
        foreach (var args in x.Predicate.Arguments.Zip(y.Predicate.Arguments, (x, y) => (x, y)))
        {
            if (!TryUnify(args.x, args.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUnify(Term x, Term y, MutableVariableSubstitution unifier)
    {
        return (x, y) switch
        {
            (VariableReference variable, _) => TryUnify(variable, y, unifier),
            (_, VariableReference variable) => TryUnify(variable, x, unifier),
            (Function functionX, Function functionY) => TryUnify(functionX, functionY, unifier),
            _ => throw new ArgumentException("Null or unsupported type of Term encountered - cannot be unified"),
        };
    }

    private static bool TryUnify(VariableReference variable, Term other, MutableVariableSubstitution unifier)
    {
        if (unifier.Bindings.TryGetValue(variable, out var value))
        {
            // The variable is already mapped to something - we need to make sure that the
            // mapping is consistent with the "other" value.
            return TryUnify(value, other, unifier);
        }
        else if (other is VariableReference otherVariable && unifier.Bindings.TryGetValue(otherVariable, out value))
        {
            // The other value is also a variable that is already mapped to something - we need to make sure that the
            // mapping is consistent with the "other" value.
            return TryUnify(variable, value, unifier);
        }
        else if (Occurs(variable, other))
        {
            return false;
        }
        else
        {
            // This substitution is not in the source book, but is so that e.g. unifying Knows(John, X) and Knows(Y, Mother(Y)) will give { X / Mother(John) }, not { X / Mother(Y) }
            // Might be duplicated effort in the broader scheme of things, but time will tell.
            other = unifier.ApplyTo(other);
            unifier.AddBinding(variable, other);
            return true;
        }
    }

    private static bool TryUnify(Function x, Function y, MutableVariableSubstitution unifier)
    {
        if (!x.Identifier.Equals(y.Identifier))
        {
            return false;
        }

        foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
        {
            if (!TryUnify(args.x, args.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool Occurs(VariableReference variable, Term term)
    {
        // Unlike the real version of this class (that uses a type switch), here we use a SentenceTransformation to carry out the occurs check:
        var occursCheck = new OccursCheck(variable);
        occursCheck.Visit(term);
        return occursCheck.IsFound;
    }

    private class OccursCheck : RecursiveSentenceVisitor
    {
        private readonly VariableReference variableReference;

        public OccursCheck(VariableReference variableReference) => this.variableReference = variableReference;

        public bool IsFound { get; private set; } = false;

        public override void Visit(VariableReference variable)
        {
            if (variable.Equals(variableReference))
            {
                // Performance might be slightly better if overrode everything to stop as soon as IsFound is true.
                // This better achieved as in production version, though.
                IsFound = true;
            }
        }
    }
}
