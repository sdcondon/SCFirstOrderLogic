// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// <para>
/// Utility class for creating unifiers that unify variable references only.
/// Useful when e.g. ordinalisation cannot be used due to the inability to establish an ordering of literals.
/// </para>
/// </summary>
internal static class VariableUnifier
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
        if (x.IsNegated != y.IsNegated)
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
            (VariableReference variableX, VariableReference variableY) => TryUpdateUnsafe(variableX, variableY, unifier),
            (Function functionX, Function functionY) => TryUpdateUnsafe(functionX, functionY, unifier),
            _ => false
        };
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(VariableReference x, VariableReference y, MutableVariableSubstitution unifier)
    {
        if (x.Equals(y))
        {
            return true;
        }
        else if (unifier.Bindings.TryGetValue(x, out var variableValue))
        {
            // The variable is already mapped to something - we need to make sure that the
            // mapping is consistent with the "other" value.
            return variableValue.Equals(y);
        }
        else
        {
            unifier.AddBinding(x, y);
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
}
