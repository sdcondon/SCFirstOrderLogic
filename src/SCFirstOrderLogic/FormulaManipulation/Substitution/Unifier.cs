// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.FormulaManipulation.Substitution;

/// <summary>
/// <para>
/// Utility class for creating unifiers - that is, variable substitutions that yield the same result when applied each of the expressions that they unify.
/// </para>
/// <para>
/// This implementation includes an occurs check.
/// </para>
/// <para>
/// See §9.2.2 ("Unification") of 'Artificial Intelligence: A Modern Approach' for an explanation of this algorithm.
/// </para>
/// </summary>
public static class Unifier
{
    /// <summary>
    /// Attempts to create the most general unifier for two literals.
    /// </summary>
    /// <param name="x">One of the two literals to attempt to create a unifier for.</param>
    /// <param name="y">One of the two literals to attempt to create a unifier for.</param>
    /// <param name="unifier">If the literals can be unified, this out parameter will be the unifier.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryCreate(Literal x, Literal y, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateUnsafe(x, y, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to create the most general unifier for two literals.
    /// </summary>
    /// <param name="x">One of the two literals to attempt to create a unifier for.</param>
    /// <param name="y">One of the two literals to attempt to create a unifier for.</param>
    /// <returns>The unifier if the literals can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryCreate(Literal x, Literal y)
    {
        var unifierAttempt = new MutableVariableSubstitution();
        return TryUpdateUnsafe(x, y, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to create the most general unifier for two predicates.
    /// </summary>
    /// <param name="x">One of the two predicates to attempt to create a unifier for.</param>
    /// <param name="y">One of the two predicates to attempt to create a unifier for.</param>
    /// <param name="unifier">If the predicates can be unified, this out parameter will be the unifier.</param>
    /// <returns>True if the two predicates can be unified, otherwise false.</returns>
    public static bool TryCreate(Predicate x, Predicate y, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateUnsafe(x, y, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to create the most general unifier for two predicates.
    /// </summary>
    /// <param name="x">One of the two predicates to attempt to create a unifier for.</param>
    /// <param name="y">One of the two predicates to attempt to create a unifier for.</param>
    /// <returns>The unifier if the predicates can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryCreate(Predicate x, Predicate y)
    {
        var unifierAttempt = new MutableVariableSubstitution();
        return TryUpdateUnsafe(x, y, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to create the most general unifier for two terms.
    /// </summary>
    /// <param name="x">One of the two terms to attempt to create a unifier for.</param>
    /// <param name="y">One of the two terms to attempt to create a unifier for.</param>
    /// <param name="unifier">If the terms can be unified, this out parameter will be the unifier.</param>
    /// <returns>True if the two terms can be unified, otherwise false.</returns>
    public static bool TryCreate(Term x, Term y, [MaybeNullWhen(false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateUnsafe(x, y, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to create the most general unifier for two terms.
    /// </summary>
    /// <param name="x">One of the two terms to attempt to create a unifier for.</param>
    /// <param name="y">One of the two terms to attempt to create a unifier for.</param>
    /// <returns>The unifier if the terms can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryCreate(Term x, Term y)
    {
        var unifierAttempt = new MutableVariableSubstitution();
        return TryUpdateUnsafe(x, y, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given literals.
    /// </summary>
    /// <param name="x">One of the two literals to attempt to unify.</param>
    /// <param name="y">One of the two literals to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <param name="updatedUnifier">Will be populated with the updated unifier on success, or be null on failure.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryUpdate(Literal x, Literal y, VariableSubstitution unifier, [MaybeNullWhen(false)] out VariableSubstitution updatedUnifier)
    {
        var potentialUpdatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(x, y, potentialUpdatedUnifier))
        {
            updatedUnifier = potentialUpdatedUnifier.CopyAsReadOnly();
            return true;
        }

        updatedUnifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given literals.
    /// </summary>
    /// <param name="x">One of the two literals to attempt to unify.</param>
    /// <param name="y">One of the two literals to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryUpdate(Literal x, Literal y, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(x, y, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given literals.
    /// </summary>
    /// <param name="x">One of the two literals to attempt to unify.</param>
    /// <param name="y">One of the two literals to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <returns>The unifier if the literals can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryUpdate(Literal x, Literal y, VariableSubstitution unifier)
    {
        var unifierAttempt = unifier.CopyAsMutable();
        return TryUpdateUnsafe(x, y, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given predicates.
    /// </summary>
    /// <param name="x">One of the two predicates to attempt to unify.</param>
    /// <param name="y">One of the two predicates to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <param name="updatedUnifier">Will be populated with the updated unifier on success, or be null on failure.</param>
    /// <returns>True if the two predicates can be unified, otherwise false.</returns>
    public static bool TryUpdate(Predicate x, Predicate y, VariableSubstitution unifier, [MaybeNullWhen(false)] out VariableSubstitution updatedUnifier)
    {
        var potentialUpdatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(x, y, potentialUpdatedUnifier))
        {
            updatedUnifier = potentialUpdatedUnifier.CopyAsReadOnly();
            return true;
        }

        updatedUnifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given predicates.
    /// </summary>
    /// <param name="x">One of the two predicates to attempt to unify.</param>
    /// <param name="y">One of the two predicates to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two predicates can be unified, otherwise false.</returns>
    public static bool TryUpdate(Predicate x, Predicate y, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(x, y, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given predicates.
    /// </summary>
    /// <param name="x">One of the two predicates to attempt to unify.</param>
    /// <param name="y">One of the two predicates to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <returns>The unifier if the predicates can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryUpdate(Predicate x, Predicate y, VariableSubstitution unifier)
    {
        var unifierAttempt = unifier.CopyAsMutable();
        return TryUpdateUnsafe(x, y, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given terms.
    /// </summary>
    /// <param name="x">One of the two terms to attempt to unify.</param>
    /// <param name="y">One of the two terms to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <param name="updatedUnifier">Will be populated with the updated unifier on success, or be null on failure.</param>
    /// <returns>True if the two terms can be unified, otherwise false.</returns>
    public static bool TryUpdate(Term x, Term y, VariableSubstitution unifier, [MaybeNullWhen(false)] out VariableSubstitution updatedUnifier)
    {
        var potentialUpdatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(x, y, potentialUpdatedUnifier))
        {
            updatedUnifier = potentialUpdatedUnifier.CopyAsReadOnly();
            return true;
        }

        updatedUnifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given terms.
    /// </summary>
    /// <param name="x">One of the two terms to attempt to unify.</param>
    /// <param name="y">One of the two terms to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two terms can be unified, otherwise false.</returns>
    public static bool TryUpdate(Term x, Term y, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(x, y, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given terms.
    /// </summary>
    /// <param name="x">One of the two terms to attempt to unify.</param>
    /// <param name="y">One of the two terms to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <returns>The unifier if the literals can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryUpdate(Term x, Term y, VariableSubstitution unifier)
    {
        var unifierAttempt = unifier.CopyAsMutable();
        return TryUpdateUnsafe(x, y, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }
    
    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Literal x, Literal y, MutableVariableSubstitution unifier)
    {
        if (x.IsNegative != y.IsNegative)
        {
            return false;
        }

        return TryUpdateUnsafe(x.Predicate, y.Predicate, unifier);
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Predicate x, Predicate y, MutableVariableSubstitution unifier)
    {
        if (!x.Identifier.Equals(y.Identifier)
            || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
        {
            if (!TryUpdateUnsafe(args.x, args.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Term x, Term y, MutableVariableSubstitution unifier)
    {
        return (x, y) switch
        {
            (VariableReference variable, _) => TryUpdateUnsafe(variable, y, unifier),
            (_, VariableReference variable) => TryUpdateUnsafe(variable, x, unifier),
            (Function functionX, Function functionY) => TryUpdateUnsafe(functionX, functionY, unifier),
            _ => throw new ArgumentException("Null or unsupported type of Term encountered - cannot be unified"),
        };
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(VariableReference variable, Term other, MutableVariableSubstitution unifier)
    {
        if (variable.Equals(other))
        {
            return true;
        }
        else if (unifier.Bindings.TryGetValue(variable, out var variableValue))
        {
            // The variable is already mapped to something - we need to make sure that the
            // mapping is consistent with the "other" value.
            return TryUpdateUnsafe(variableValue, other, unifier);
        }
        else if (other is VariableReference otherVariable && unifier.Bindings.TryGetValue(otherVariable, out var otherVariableValue))
        {
            // The other value is also a variable that is already mapped to something - we need to make sure that the
            // mapping is consistent with the "other" value.
            return TryUpdateUnsafe(variable, otherVariableValue, unifier);
        }
        else if (Occurs(variable, unifier.ApplyTo(other)))
        {
            // We'd be adding a cycle if we added this binding - so fail instead.
            return false;
        }
        else
        {
            unifier.AddBinding(variable, other);
            return true;
        }
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Function x, Function y, MutableVariableSubstitution unifier)
    {
        if (!x.Identifier.Equals(y.Identifier) || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Arguments.Count; i++)
        {
            if (!TryUpdateUnsafe(x.Arguments[i], y.Arguments[i], unifier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool Occurs(VariableReference variableReference, Term term)
    {
        return term switch
        {
            VariableReference v => variableReference.Equals(v),
            Function f => f.Arguments.Any(a => Occurs(variableReference, a)),
            _ => throw new ArgumentException($"Unexpected term type '{term.GetType()}' encountered", nameof(term)),
        };
    }
}
